//Note: download the mqttnet library
//Add the MQTTnet.dll to your Unity project's Assets/Plugins folder

using System;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MqttSubscriber : MonoBehaviour
{
    private IMqttClient mqttClient;

    // MQTT Broker details
    private string MQTT_BROKER = "broker.hivemq.com";
    private int MQTT_PORT = 1883;
    private string MQTT_TOPIC = "mediapipe/pose_data";

    void Start()
    {
        // Create MQTT client
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        // Configure options
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(MQTT_BROKER, MQTT_PORT)
            .Build();

        // Connect to broker
        mqttClient.ConnectAsync(options).Wait();
        Debug.Log("✅ Connected to MQTT Broker");

        // Subscribe to topic
        mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(MQTT_TOPIC).Build()).Wait();
        Debug.Log($"📥 Subscribed to topic: {MQTT_TOPIC}");

        // Register message handler
        mqttClient.UseApplicationMessageReceivedHandler(OnMessageReceived);
    }

    private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        Debug.Log($"📩 Received message: {payload}");

        // Deserialize JSON data
        try
        {
            var poseData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(payload);

            // Print pose data to the console
            foreach (var landmark in poseData)
            {
                string landmarkName = landmark.Key;
                float x = Convert.ToSingle(landmark.Value["x"]);
                float y = Convert.ToSingle(landmark.Value["y"]);
                float z = Convert.ToSingle(landmark.Value["z"]);
                string movementState = landmark.Value["movement"].ToString();

                Debug.Log($"{landmarkName}: X={x}, Y={y}, Z={z}, Movement={movementState}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to parse JSON: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        // Disconnect MQTT client
        if (mqttClient != null && mqttClient.IsConnected)
        {
            mqttClient.DisconnectAsync().Wait();
            Debug.Log("✅ Disconnected from MQTT Broker");
        }
    }
}