import { HubConnectionBuilder } from "@microsoft/signalr";
import eventBus from "./eventBus";

class SignalRHub {
  constructor() {
    this.client = new HubConnectionBuilder()
      .withUrl("http://localhost:7136/playerHub")
      .build();

    this.client.onreconnected(this.onReconnected.bind(this));
    this.client.onclose(this.onDisconnected.bind(this));
    this.isConnected = false;
  }

  start() {
    this.client
      .start()
      .then(() => this.onConnected())
      .catch((error) => {
        console.error("Error starting SignalR connection: ", error);
        setTimeout(() => this.start(), 5000);
      });
  }

  stop() {
    this.client
      .stop()
      .then((error) => this.onDisconnected(error))
      .catch((error) => {
        console.error("Error stopping SignalR connection: ", error);
      });
  }

  invoke(methodeName, ...argument) {
    if (this.isConnected) this.client.invoke(methodeName, ...argument);
  }

  on(methodeName, callback) {
    this.client.on(methodeName, callback);
  }

  invokeWithNoData(methodeName) {
    this.client.invoke(methodeName);
  }

  onReconnected(connectionId) {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR reconneected to server ", connectionId);
  }

  onConnected() {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR connected to server");
  }

  onDisconnected(error) {
    this.isConnected = false;
    console.log("SignalR disconnected", error);
    eventBus.emit("signalr-connected", false);
    this.start();
  }
}

export default new SignalRHub();
