#include <esp_now.h>
#include <WiFi.h>

// Structure to receive data
// Must match the sender structure
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

// Updated callback function for ESP32 Arduino core 3.x
void OnDataRecv(const esp_now_recv_info_t *info, const uint8_t *incomingData, int len) {
  // Print sender MAC address
  char macStr[18];
  snprintf(macStr, sizeof(macStr), "%02X:%02X:%02X:%02X:%02X:%02X",
           info->src_addr[0], info->src_addr[1], info->src_addr[2], 
           info->src_addr[3], info->src_addr[4], info->src_addr[5]);
  Serial.print("Received from: ");
  Serial.println(macStr);
  
  memcpy(&sensorData, incomingData, sizeof(sensorData));
  Serial.print("Bytes received: ");
  Serial.println(len);
  
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
  
  Serial.println("----------------------------");
}
 
void setup() {
  // Initialize Serial Monitor
  Serial.begin(115200);
  
  // Set device as a Wi-Fi Station
  WiFi.mode(WIFI_STA);

  // Init ESP-NOW
  if (esp_now_init() != ESP_OK) {
    Serial.println("Error initializing ESP-NOW");
    return;
  }
  
  // Register for recv callback to get received packets
  esp_now_register_recv_cb(OnDataRecv);
  
  Serial.println("ESP-NOW Receiver initialized and ready to receive data");
}
 
void loop() {
  // Nothing to do here, everything happens in the callback function
  delay(10);  // Small delay to prevent watchdog timer issues
}