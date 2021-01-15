/*
  Name:		RemoteLamp.ino
  Created:	1/9/202 12:00:00 PM
  Author:	Mr.BiggyBear
  Updated:   1/15/2021
*/

#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266mDNS.h>

#include <ESP8266WebServer.h> // Include the WebServer library

#include "shh.h"

ESP8266WebServer server(80);  // Create a webserver object that listens for HTTP request on port 80
MDNSResponder dns_res;

bool isLampOn = false;

using namespace Info;
using namespace Network;


void setupEndpoints()
{
  // setup endpoints
  Serial.println("setting up endpoints...");

  server.on("/", [&]() {
    String msg = "Hello from " + String(deviceName);
    server.send(200, "text", msg + String(deviceName));
  });

  server.on("/status", [&]() {
    String msg = "Hello from " + String(deviceName);
    server.send(200, "text", isLampOn ? "true" : "false");
  });

  server.on("/on", [&]() {
    // set lamp state to true
    isLampOn = true;

    digitalWrite(2, LOW);
    digitalWrite(16, LOW);

    String msg = "Lamp is on...";
    server.send(200, "text/plain", msg);
  });
  server.on("/off", [&]() {
    isLampOn = false;

    digitalWrite(2, HIGH);
    digitalWrite(16, HIGH);

    String msg = "Lamp is off...";
    server.send(200, "text/plain", msg);
  });

  //    server.onNotFound(
  //            handleNotFound(); // When a client requests an unknown URI (i.e. something other than "/"), call function "handleNotFound"
}

void setup()
{
  if (!Serial) {
    Serial.begin(BAUD);
    //    Serial1.begin(BAUD);
  }

  pinMode(16, OUTPUT);
  pinMode(2, OUTPUT);

  WiFi.begin(SSID, PASSWORD);
  WiFi.mode(WIFI_STA);

  Serial.println("Connecting to WiFi.");
  if (WiFi.status() != WL_CONNECTED) {
    WiFi.hostname(String(deviceName));
    delay(500);
    Serial.print(".");
  }
  if (dns_res.begin(deviceName, WiFi.localIP())) {
    Serial.println("\n\nMDNS responder started");
  }
  Serial.read();
  setupEndpoints();
  Serial.println("\nConnected to WiFi");

  Serial.println("<Node " + String(deviceName) + " is ready>" +
                 "Connected to " + String(SSID) + " @ "
                );
  Serial.println(WiFi.localIP());

  // Start the server
  server.begin();
  Serial.println("Server started");

  digitalWrite(2, LOW);
  digitalWrite(16, LOW);
}

void loop() {
  server.handleClient();
}
