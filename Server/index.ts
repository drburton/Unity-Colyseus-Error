import http from "http";
import express from "express";
import cors from "cors";

import { Server } from "colyseus";
import { RelayRoom } from "colyseus";
//import { monitor } from "@colyseus/monitor"
import { DemoRoom } from "./rooms/DemoRoom";
import { TankRoomAlpha } from "./rooms/TankRoomAlpha";

const PORT = Number(process.env.PORT || 2567);
const app = express();

/**
 * CORS should be used during development only.
 * Please remove CORS on production, unless you're hosting the server and client on different domains.
 */
app.use(cors());
app.use(express.json())

const server = http.createServer(app);
const gameServer = new Server({
  server,
});

// Register room handlers
gameServer.define("demo", DemoRoom);
gameServer.define("tanka", TankRoomAlpha);
gameServer.define("relay", RelayRoom, {maxClients: 4, allowReconnectionTime: 120});


// register colyseus monitor AFTER registering your room handlers
//app.use("/colyseus", monitor());

// Listen on specified PORT number
gameServer.listen(PORT);

console.log("Running on ws://localhost:" + PORT);
