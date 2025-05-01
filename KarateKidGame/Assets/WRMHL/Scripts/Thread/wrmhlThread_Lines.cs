using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // For Exception types

public class wrmhlThread_Lines : wrmhlThread {

    public wrmhlThread_Lines(string portName, int baudRate, int readTimeout, int QueueLength) 
        : base(portName, baudRate, readTimeout, QueueLength) {
    }

    public wrmhlThread_Lines(string portName, int baudRate) 
        : base(portName, baudRate) {
    }

    public override string ReadProtocol() {
        try {
            if (deviceSerial != null && deviceSerial.IsOpen && deviceSerial.BytesToRead > 0) {
                return deviceSerial.ReadLine();
            }
        } catch (TimeoutException) {
            Debug.LogWarning("Serial read timed out.");
        } catch (Exception e) {
            Debug.LogError("Unexpected serial read error: " + e.Message);
        }
        return null;
    }

    public override void SendProtocol(object message) {
        try {
            if (deviceSerial != null && deviceSerial.IsOpen) {
                deviceSerial.WriteLine((string)message);
            }
        } catch (Exception e) {
            Debug.LogError("Serial write error: " + e.Message);
        }
    }
}
