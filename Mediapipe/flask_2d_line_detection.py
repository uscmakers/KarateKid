import cv2
import threading
import numpy as np
import mediapipe as mp
from collections import deque
from flask import Flask, jsonify, Response

app = Flask(__name__)

# Shared storage for wrist data
latest_data = {
    'frame': None,
    'wrist_position': None,
    'movement_detected': False,
    'x_movement': 0
}
lock = threading.Lock()

# MediaPipe Setup
mp_pose = mp.solutions.pose
pose = mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5)

# Camera Setup
cap = cv2.VideoCapture(0)
if not cap.isOpened():
    print("❌ Failed to access camera.")
    exit()

# Store previous wrist positions (deque keeps the last 2 positions)
wrist_positions = deque(maxlen=2)
MOVEMENT_THRESHOLD = 300  # pixels

def generate_frames():
    while True:
        with lock:
            if latest_data['frame'] is not None:
                ret, buffer = cv2.imencode('.jpg', latest_data['frame'])
                frame = buffer.tobytes()
                yield (b'--frame\r\n'
                       b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')

def process_frame_loop():
    global latest_data
    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                print("❌ Failed to capture frame.")
                continue

            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False
            results = pose.process(image)
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            movement_detected = False
            x_movement = 0
            wrist_position = None

            if results.pose_landmarks:
                landmarks = results.pose_landmarks.landmark

                # Extract right wrist coordinates
                right_wrist = landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value]
                wrist_coords = (int(right_wrist.x * frame.shape[1]), 
                                int(right_wrist.y * frame.shape[0]))
                wrist_position = {'x': wrist_coords[0], 'y': wrist_coords[1]}

                # Store the current wrist position
                wrist_positions.append(wrist_coords)

                # Check for significant horizontal movement
                if len(wrist_positions) == 2:
                    x_movement = abs(wrist_positions[1][0] - wrist_positions[0][0])
                    movement_detected = x_movement >= MOVEMENT_THRESHOLD

                # Draw visualization
                cv2.circle(image, wrist_coords, 10, (0, 255, 0), -1)
                cv2.putText(image, f"Position: {wrist_coords}", (10, 30),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)
                cv2.putText(image, f"X Movement: {x_movement}", (10, 60),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, 
                            (0, 255, 0) if movement_detected else (0, 0, 255), 2)
                
                if movement_detected:
                    cv2.putText(image, "LINE DETECTED!", (frame.shape[1]//2 - 100, 50),
                                cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

            with lock:
                latest_data.update({
                    'frame': image,
                    'wrist_position': wrist_position,
                    'movement_detected': movement_detected,
                    'x_movement': x_movement
                })

    except Exception as e:
        print(f"🚨 Error in frame processing: {str(e)}")
    finally:
        cap.release()
        cv2.destroyAllWindows()
        print("✅ Camera resources released")

@app.route('/')
def index():
    return """
    <html>
        <head>
            <title>2D Line Detection</title>
            <style>
                body { font-family: Arial, sans-serif; text-align: center; margin-top: 20px; }
                h1 { color: #333; }
                .container { max-width: 800px; margin: 0 auto; }
            </style>
        </head>
        <body>
            <div class="container">
                <h1>2D Line Detection (Right Wrist Tracking)</h1>
                <img src="/video_feed" width="640" height="480">
                <p>
                    <a href="/wrist_data" style="display: inline-block; margin-top: 20px; 
                        padding: 10px 15px; background: #4CAF50; color: white; 
                        text-decoration: none; border-radius: 5px;">
                        Get Wrist Data JSON
                    </a>
                </p>
            </div>
        </body>
    </html>
    """

@app.route('/video_feed')
def video_feed():
    return Response(generate_frames(), 
                    mimetype='multipart/x-mixed-replace; boundary=frame')

@app.route('/wrist_data', methods=['GET'])
def get_wrist_data():
    with lock:
        response_data = {
            'success': latest_data['wrist_position'] is not None,
            'position': latest_data['wrist_position'],
            'movement_detected': latest_data['movement_detected'],
            'x_movement': latest_data['x_movement'],
            'threshold': MOVEMENT_THRESHOLD,
            'timestamp': time.time()
        }
        return jsonify(response_data)

if __name__ == '__main__':
    # Start the camera processing thread
    threading.Thread(target=process_frame_loop, daemon=True).start()
    
    # Start Flask server
    app.run(host='0.0.0.0', port=5051, debug=False, threaded=True)