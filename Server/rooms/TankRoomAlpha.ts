import { Room, Client, generateId } from "colyseus";
import { Schema, MapSchema, ArraySchema, Context } from "@colyseus/schema";
import { verifyToken, User, IUser } from "@colyseus/social";
import { Console } from "console";

// Create a context for this room's state data.
const type = Context.create();

class Entity extends Schema {
    @type("string") id: string;
    @type("string") ownerId: string;
    @type("number") xPos: number = 0;
    @type("number") yPos: number = 0;
    @type("number") zPos: number = 0;
    @type("number") xRot: number = 0;
    @type("number") yRot: number = 0;
    @type("number") zRot: number = 0;
    @type("number") wRot: number = 0;
}

class Player extends Entity {
    @type("string") sessionId: string;
    @type("boolean") connected: boolean;
//    @type("number") timestamp: number;
}

class State extends Schema {
  @type({ map: Entity }) entities = new MapSchema<Entity>();
  @type({ map: Player}) players = new MapSchema<Player>(); 
}

/**
 * Demonstrate sending schema data types as messages
*/
class Message extends Schema {
  @type("number") num;
  @type("string") str;
}
 

export class TankRoomAlpha extends Room {

  onCreate (options: any) {
    console.log("TankRoom created.", options);

    this.setState(new State());
  
    this.setPatchRate(1000 / 20);

    this.onMessage("move", (client, message) => {
      //handle xPos,yPos,zPos, xRot,yRot,zRot
      console.log("move: client, message: ", client, message);
    });

/*
    this.onMessage("move_right", (client) => {
      this.state.entities[client.sessionId].x = 0;
      this.state.entities[client.sessionId].y = 0;
      this.state.entities[client.sessionId].x += 0.01;
    }
*/
  }
/*
  async onAuth (client, options) {
      console.log("onAuth(), options!", options);
      return await User.findById(verifyToken(options.token)._id);
    }
*/
    onJoin (client: Client, options: any, user: IUser) {
        console.log("client joined!", client.sessionId);
        this.state.entities[client.sessionId] = new Player();

        client.send("type", { hello: true }); //should send client full schema?
    }

  async onLeave (client: Client, consented: boolean) {
    this.state.entities[client.sessionId].connected = false;

    try {
      if (consented) {
        throw new Error("consented leave!");
      }

      console.log("let's wait for reconnection!")
      const newClient = await this.allowReconnection(client, 10);
      console.log("reconnected!", newClient.sessionId);

    } catch (e) {
      console.log("disconnected!", client.sessionId);
      delete this.state.entities[client.sessionId];
    }
  }

/*
  update (dt?: number) {
    // console.log("num clients:", Object.keys(this.clients).length);
  }
*/
/*
  onDispose () {
    console.log("DemoRoom disposed.");
  }
*/
}
