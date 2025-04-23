
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

// Create a structure to hold the readings from each sender
typedef struct sensor_readings {
  uint8_t mac_addr[6];
  struct_message data;
  bool newData;
} sensor_readings;

// Array to store data from two sensors
sensor_readings sensorReadings[2] = {0};

// Initialize with unknown MAC addresses
void initializeSensorReadings() {
  for (int i = 0; i < 2; i++) {
    memset(sensorReadings[i].mac_addr, 0, sizeof(sensorReadings[i].mac_addr));
    sensorReadings[i].newData = false;
  }
}

// Check if the MAC address is known and return its index
int getMacIndex(const uint8_t* mac_addr) {
  // First, check if we know this MAC
  for (int i = 0; i < 2; i++) {
    bool known = true;
    for (int j = 0; j < 6; j++) {
      if (sensorReadings[i].mac_addr[j] != 0 && sensorReadings[i].mac_addr[j] != mac_addr[j]) {
        known = false;
        break;
      }
    }
    if (known) {
      // If MAC is zeroed out (new), save this MAC
      if (sensorReadings[i].mac_addr[0] == 0) {
        memcpy(sensorReadings[i].mac_addr, mac_addr, 6);
        Serial.print("New sender registered at index ");
        Serial.println(i);
      }
      return i;
    }
  }
  
  // If we get here, try to find an empty slot
  for (int i = 0; i < 2; i++) {
    if (sensorReadings[i].mac_addr[0] == 0) {
      memcpy(sensorReadings[i].mac_addr, mac_addr, 6);
      Serial.print("New sender registered at index ");
      Serial.println(i);
      return i;
    }
  }
  
  // If we get here, no available slots
  return -1;
}

// Function to print MAC address
void printMAC(const uint8_t* mac_addr) {
  char macStr[18];
  snprintf(macStr, sizeof(macStr), "%02X:%02X:%02X:%02X:%02X:%02X",
           mac_addr[0], mac_addr[1], mac_addr[2], 
           mac_addr[3], mac_addr[4], mac_addr[5]);
  Serial.print(macStr);
}

// Updated callback function for ESP32 Arduino core 3.x
void OnDataRecv(const esp_now_recv_info_t *info, const uint8_t *incomingData, int len) {
  // Identify which sender this is
  int senderIndex = getMacIndex(info->src_addr);
  
  if (senderIndex >= 0) {
    // Copy the data into the correct slot
    memcpy(&sensorReadings[senderIndex].data, incomingData, sizeof(struct_message));
    sensorReadings[senderIndex].newData = true;
    
    // Print sender info
    // Serial.print("Received from: ");
    printMAC(info->src_addr);
    // Serial.print(" (Sensor ");
    Serial.print(senderIndex + 1);
    // Serial.println(")");
  } else {
    Serial.println("Error: Too many senders or cannot identify sender");
    return;
  }
  
  // Print the received data
  // Serial.print("Bytes received: ");
  // Serial.println(len);
  
  struct_message &sensorData = sensorReadings[senderIndex].data;
  
  Serial.print(" ");
  Serial.print(sensorData.angleX);
  Serial.print(" ");
  Serial.print(sensorData.angleY);
  Serial.print(" ");
  Serial.println(sensorData.angleZ);
  
  // Serial.print("Acc X: ");
  // Serial.print(sensorData.accX);
  // Serial.print("\tAcc Y: ");
  // Serial.print(sensorData.accY);
  // Serial.print("\tAcc Z: ");
  // Serial.println(sensorData.accZ);
  
  // Serial.println("----------------------------");
}

// Function to display all sensor data periodically
void displayAllSensorData() {
  Serial.println("\n========== CURRENT SENSOR READINGS ==========");
  
  for (int i = 0; i < 2; i++) {
    if (sensorReadings[i].mac_addr[0] != 0) {
      Serial.print("Sensor ");
      Serial.print(i + 1);
      Serial.print(" (MAC: ");
      printMAC(sensorReadings[i].mac_addr);
      Serial.println(")");
      
      if (sensorReadings[i].newData) {
        struct_message &data = sensorReadings[i].data;
        Serial.print("Angle X: ");
        Serial.print(data.angleX);
        Serial.print("\tAngle Y: ");
        Serial.print(data.angleY);
        Serial.print("\tAngle Z: ");
        Serial.println(data.angleZ);
        
        Serial.print("Acc X: ");
        Serial.print(data.accX);
        Serial.print("\tAcc Y: ");
        Serial.print(data.accY);
        Serial.print("\tAcc Z: ");
        Serial.println(data.accZ);
      } else {
        Serial.println("No data received yet");
      }
      Serial.println();
    }
  }
  
  Serial.println("=============================================");
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
  
  // Initialize the sensor readings array
  initializeSensorReadings();
  
  // Register for recv callback to get received packets
  esp_now_register_recv_cb(OnDataRecv);
  
  Serial.println("ESP-NOW Multi-Sensor Receiver initialized");
  Serial.println("Ready to receive data from up to 2 senders");
}

unsigned long lastDisplayTime = 0;
const unsigned long displayInterval = 5000; // Display summary every 5 seconds
 
void loop() {
  // // Periodically display all sensor data
  // unsigned long currentTime = millis();
  // if (currentTime - lastDisplayTime >= displayInterval) {
  //   displayAllSensorData();
  //   lastDisplayTime = currentTime;
  // }
  
  // delay(10);  // Small delay to prevent watchdog timer issues
}
