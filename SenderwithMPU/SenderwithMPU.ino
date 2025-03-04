/*
  Combined MPU6050 reading with ESP-NOW communication
  Reads accelerometer and gyroscope data from MPU6050 and sends it to a receiver ESP32
*/

#include <MPU6050_tockn.h>
#include <Wire.h>
#include <esp_now.h>
#include <WiFi.h>

// Replace with your receiver's MAC Address
uint8_t broadcastAddress[] = {0x90, 0x15, 0x06, 0x93, 0xf2, 0x1c};

MPU6050 mpu6050(Wire);

// Structure to send data - must match the receiver structure
typedef struct struct_message {
  float angleX;
  float angleY;
  float angleZ;
  float accX;
  float accY;
  float accZ;
} struct_message;

// Create a struct_message called sensorData
struct_message sensorData;
esp_now_peer_info_t peerInfo;

// Callback function executed when data is sent
void OnDataSent(const uint8_t *mac_addr, esp_now_send_status_t status) {
  Serial.print("Last Packet Send Status: ");
  Serial.println(status == ESP_NOW_SEND_SUCCESS ? "Delivery Success" : "Delivery Fail");
}

void setup() {
  // Initialize Serial Monitor
  Serial.begin(115200);
  
  // Initialize I2C communication
  Wire.begin();
  
  // Initialize MPU6050
  mpu6050.begin();
  Serial.println("Calibrating gyroscope...");
  mpu6050.calcGyroOffsets(true);  // Set true to see calibration results in Serial Monitor
  Serial.println("MPU6050 initialized");
  
  // Set device as a Wi-Fi Station
  WiFi.mode(WIFI_STA);
  
  // Initialize ESP-NOW
  if (esp_now_init() != ESP_OK) {
    Serial.println("Error initializing ESP-NOW");
    return;
  }
  
  // Register callback function
  esp_now_register_send_cb(OnDataSent);
  
  // Register peer
  memcpy(peerInfo.peer_addr, broadcastAddress, 6);
  peerInfo.channel = 0;  
  peerInfo.encrypt = false;
  
  // Add peer        
  if (esp_now_add_peer(&peerInfo) != ESP_OK) {
    Serial.println("Failed to add peer");
    return;
  }
  
  Serial.println("ESP-NOW initialized and peer added");
}

void loop() {
  // Update MPU6050 data
  mpu6050.update();
  
  // Get sensor values
  sensorData.angleX = mpu6050.getAngleX();
  sensorData.angleY = mpu6050.getAngleY();
  sensorData.angleZ = mpu6050.getAngleZ();
  sensorData.accX = mpu6050.getAccX();
  sensorData.accY = mpu6050.getAccY();
  sensorData.accZ = mpu6050.getAccZ();
  
  // Print values to Serial Monitor
  Serial.print("Angle X: ");
  Serial.print(sensorData.angleX);
  Serial.print("\tAngle Y: ");
  Serial.print(sensorData.angleY);
  Serial.print("\tAngle Z: ");
  Serial.println(sensorData.angleZ);
  
  Serial.print("Acc X: ");
  Serial.print(sensorData.accX);
  Serial.print("\tAcc Y: ");
  Serial.print(sensorData.accY);
  Serial.print("\tAcc Z: ");
  Serial.println(sensorData.accZ);
  
  // Send message via ESP-NOW
  esp_err_t result = esp_now_send(broadcastAddress, (uint8_t *)&sensorData, sizeof(sensorData));
  
  if (result == ESP_OK) {
    Serial.println("Data sent successfully");
  } else {
    Serial.println("Error sending data");
  }
  
  Serial.println("----------------------------");
  delay(150);  // Send data every 150ms
}