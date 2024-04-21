var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
import { HubConnectionBuilder } from "@microsoft/signalr";
import eventBus from "./eventBus";
var SignalRHub = /** @class */ (function () {
    function SignalRHub() {
        this.client = new HubConnectionBuilder()
            .withUrl("http://localhost:7136/playerHub")
            .build();
        this.client.onreconnected(this.onReconnected.bind(this));
        this.client.onclose(this.onDisconnected.bind(this));
        this.isConnected = false;
    }
    SignalRHub.prototype.start = function () {
        var _this = this;
        this.client
            .start()
            .then(function () { return _this.onConnected(); })
            .catch(function (error) {
            console.error("Error starting SignalR connection: ", error);
            setTimeout(function () { return _this.start(); }, 5000);
        });
    };
    SignalRHub.prototype.stop = function () {
        var _this = this;
        this.client
            .stop()
            .then(function () { return _this.onDisconnected(); })
            .catch(function (error) {
            console.error("Error stopping SignalR connection: ", error);
        });
    };
    SignalRHub.prototype.invoke = function (methodeName) {
        var _a;
        var argument = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            argument[_i - 1] = arguments[_i];
        }
        if (this.isConnected)
            (_a = this.client).invoke.apply(_a, __spreadArray([methodeName], argument, false));
    };
    SignalRHub.prototype.on = function (methodeName, callback) {
        this.client.on(methodeName, callback);
    };
    SignalRHub.prototype.invokeWithNoData = function (methodeName) {
        this.client.invoke(methodeName);
    };
    SignalRHub.prototype.onReconnected = function (connectionId) {
        this.isConnected = true;
        eventBus.emit("signalr-connected", true);
        console.log("SignalR reconneected to server ", connectionId);
    };
    SignalRHub.prototype.onConnected = function () {
        this.isConnected = true;
        eventBus.emit("signalr-connected", true);
        console.log("SignalR connected to server");
    };
    SignalRHub.prototype.onDisconnected = function () {
        this.isConnected = false;
        console.log("SignalR disconnected");
        eventBus.emit("signalr-connected", false);
        this.start();
    };
    return SignalRHub;
}());
export default new SignalRHub();
