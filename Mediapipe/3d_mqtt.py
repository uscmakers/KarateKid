import paho.mqtt.client as mqtt
import mediapipe as mp
import cv2
import numpy as np
from collections import deque
import json

# MQTT Broker details
MQTT_BROKER = "broker.hivemq.com"  # Public broker for testing
MQTT_PORT = 1883
MQTT_TOPIC = "mediapipe/pose_data"

# Initialize MediaPipe Pose module
mp_pose = mp.solutions.pose
mp_drawing = mp.solutions.drawing_utils

# Open the camera (1 for external webcam, 0 for internal)
cap = cv2.VideoCapture(1)

# Check if the camera is accessible
if not cap.isOpened():
    print("❌ Failed to access camera.")
    exit()

# Movement tracking parameters
history_length = 10
MIN_MOVEMENT_THRESHOLD = 30

# Deques to store positions for each landmark
right_wrist_positions = deque(maxlen=history_length)
left_wrist_positions = deque(maxlen=history_length)
right_elbow_positions = deque(maxlen=history_length)
left_elbow_positions = deque(maxlen=history_length)
right_shoulder_positions = deque(maxlen=history_length)
left_shoulder_positions = deque(maxlen=history_length)
right_ankle_positions = deque(maxlen=history_length)
left_ankle_positions = deque(maxlen=history_length)
right_knee_positions = deque(maxlen=history_length)
left_knee_positions = deque(maxlen=history_length)

# Initialize MQTT client
client = mqtt.Client()
client.connect(MQTT_BROKER, MQTT_PORT, 60)
print(f"✅ Connected to MQTT Broker: {MQTT_BROKER}")

# Start pose detection
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                print("❌ Failed to capture frame.")
                break

            # Convert image to RGB (required for MediaPipe)
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False

            # Process image with MediaPipe Pose
            results = pose.process(image)

            # Convert back to BGR for OpenCV
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            if results.pose_landmarks:
                landmarks = results.pose_landmarks.landmark

                # Function to extract and display coordinates for a landmark
                def get_landmark_coords(landmark, landmark_name, frame, y_offset, positions_deque):
                    coords = (
                        int(landmark.x * frame.shape[1]),  # X (horizontal)
                        int(landmark.y * frame.shape[0]),  # Y (vertical)
                        int(landmark.z * 1000)  # Z (depth, scaled to match X and Y units)
                    )
                    print(f"{landmark_name} 3D Coordinates: X={coords[0]}, Y={coords[1]}, Z={coords[2]}")

                    # Map Z depth to color intensity (closer = brighter)
                    depth_color = int((1 - min(1.0, abs(landmark.z))) * 255)
                    color = (0, depth_color, 255 - depth_color)  # Color gradient based on Z

                    # Draw marker with depth-based color
                    cv2.circle(image, (coords[0], coords[1]), 10, color, -1)

                    # Track movement
                    positions_deque.append((coords[0], coords[1]))
                    movement_state = "Not Moving"
                    movement_color = (0, 255, 255)  # Yellow for not moving

                    if len(positions_deque) >= 2:
                        x_initial, y_initial = positions_deque[0]
                        x_current, y_current = positions_deque[-1]
                        total_movement = np.sqrt((x_current - x_initial) ** 2 + (y_current - y_initial) ** 2)

                        if total_movement > MIN_MOVEMENT_THRESHOLD:
                            movement_state = "Moving"
                            movement_color = (0, 255, 0)  # Green for moving

                    # Display coordinates and movement state on screen
                    cv2.putText(image, f"{landmark_name}: X={coords[0]}, Y={coords[1]}, Z={coords[2]} ({movement_state})",
                                (10, y_offset), cv2.FONT_HERSHEY_SIMPLEX, 0.6, movement_color, 2)
                    return coords

                # List of landmarks to track
                landmark_names = [
                    ("Right Wrist", mp_pose.PoseLandmark.RIGHT_WRIST, right_wrist_positions),
                    ("Left Wrist", mp_pose.PoseLandmark.LEFT_WRIST, left_wrist_positions),
                    ("Right Elbow", mp_pose.PoseLandmark.RIGHT_ELBOW, right_elbow_positions),
                    ("Left Elbow", mp_pose.PoseLandmark.LEFT_ELBOW, left_elbow_positions),
                    ("Right Shoulder", mp_pose.PoseLandmark.RIGHT_SHOULDER, right_shoulder_positions),
                    ("Left Shoulder", mp_pose.PoseLandmark.LEFT_SHOULDER, left_shoulder_positions),
                    ("Right Ankle", mp_pose.PoseLandmark.RIGHT_ANKLE, right_ankle_positions),
                    ("Left Ankle", mp_pose.PoseLandmark.LEFT_ANKLE, left_ankle_positions),
                    ("Right Knee", mp_pose.PoseLandmark.RIGHT_KNEE, right_knee_positions),
                    ("Left Knee", mp_pose.PoseLandmark.LEFT_KNEE, left_knee_positions)
                ]

                # Prepare data to send
                pose_data = {}
                y_offset = 40  # Starting y position for the first landmark
                for name, landmark, positions_deque in landmark_names:
                    landmark_point = landmarks[landmark.value]
                    if landmark_point.visibility < 0.6:
                        cv2.putText(image, f"{name} not visible!", (10, y_offset), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 0, 255), 2)
                    else:
                        coords = get_landmark_coords(landmark_point, name, frame, y_offset, positions_deque)
                        pose_data[name] = {
                            "x": coords[0],
                            "y": coords[1],
                            "z": coords[2],
                            "movement": "Moving" if len(positions_deque) >= 2 and np.sqrt(
                                (positions_deque[-1][0] - positions_deque[0][0]) ** 2 + (
                                                      positions_deque[-1][1] - positions_deque[0][1]) ** 2 > MIN_MOVEMENT_THRESHOLD else "Not Moving"
                        }
                    y_offset += 30  # Increment y position for the next landmark

                # Publish pose data as JSON
                client.publish(MQTT_TOPIC, json.dumps(pose_data))
                print("📤 Published pose data to MQTT")

            else:
                print("⚠️ No landmarks detected.")
                cv2.putText(image, "No landmarks detected!", (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

            # Show the processed video feed
            cv2.imshow("Pose Landmarks Tracking", image)

            # Exit when 'q' is pressed
            if cv2.waitKey(1) & 0xFF == ord("q"):
                break

    except KeyboardInterrupt:
        print("\n⛔ Interrupted! Releasing camera.")

# Cleanup
cap.release()
cv2.destroyAllWindows()
client.disconnect()
print("✅ Camera released and MQTT disconnected.")