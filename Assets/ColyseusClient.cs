using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Colyseus;
using Colyseus.Schema;

using GameDevWare.Serialization;

[Serializable]
class Metadata
{
	public int number;
	public string str;
}

[Serializable]
class CustomRoomAvailable : RoomAvailable
{
	public Metadata metadata;
}
[Serializable]
class MoveData
{
	public float xPos;
	public float yPos;
	public float zPos;
	public float xRot;
	public float yRot;
	public float zRot;
	public float wRot;
}

class TypeMessage
{
	public bool hello;
}

enum MessageType {
	ONE = 0
};
class MessageByEnum
{
	public string str;
}


public class ColyseusClient : MonoBehaviour {

	public String m_EndpointField = "ws://localhost:2567";
	public String m_IdText, m_SessionIdText;

	public string roomName = "tanka";
	
    public GameObject myPlayer;
	public GameObject enemy;
	public GameObject spawnPoint1;
	public GameObject spawnPoint2;
	public GameObject spawnPoint3;
	public GameObject spawnPoint4;
	public GameObject spawnPoint5;
	public GameObject spawnPoint6;
	public GameObject spawnPoint7;
	public GameObject spawnPoint8;
	public GameObject spawnPoint9;
	public GameObject spawnPoint10;
	public GameObject spawnPoint11;
	public GameObject spawnPoint12;
	public GameObject spawnPoint13;
	public GameObject spawnPoint14;
	public GameObject spawnPoint15;
	public GameObject spawnPoint16;
	public GameObject spawnPoint17;
	public GameObject spawnPoint18;

	protected Client client;
	protected Room<State> room;

	protected Room<IndexedDictionary<string, object>> roomFossilDelta;
	protected Room<object> roomNoneSerializer;

	protected IndexedDictionary<Entity, GameObject> entities = new IndexedDictionary<Entity, GameObject>();

	// Use this for initialization
	void Start () {

		ConnectToServer();
	}

	async void ConnectToServer ()
	{
		/*
		 * Get Colyseus endpoint from InputField
		 */
		string endpoint = m_EndpointField;

		Debug.Log("Connecting to " + endpoint);

		/*
		 * Connect into Colyeus Server
		 */
		client = ColyseusManager.Instance.CreateClient(endpoint);
		JoinOrCreateRoom();
	}

	public async void CreateRoom()  //NOT CURRENTLY USED
	{
		room = await client.Create<State>(roomName, new Dictionary<string, object>() { });
		//roomNoneSerializer = await client.Create("no_state", new Dictionary<string, object>() { });
		//roomFossilDelta = await client.Create<IndexedDictionary<string, object>>("fossildelta", new Dictionary<string, object>() { });
		
		RegisterRoomHandlers();
	}

	public async void JoinOrCreateRoom()
	{
		room = await client.JoinOrCreate<State>(roomName, new Dictionary<string, object>() { });
		
//		Debug.Log(room);
		RegisterRoomHandlers();
	}

	public async void JoinRoom ()  //Not Currently Used
	{
		room = await client.Join<State>(roomName, new Dictionary<string, object>() { });
		RegisterRoomHandlers();
	}

	async void ReconnectRoom ()
	{
		string roomId = PlayerPrefs.GetString("roomId");
		string sessionId = PlayerPrefs.GetString("sessionId");
		if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(roomId))
		{
			Debug.Log("Cannot Reconnect without having a roomId and sessionId");
			return;
		}

		room = await client.Reconnect<State>(roomId, sessionId);

		Debug.Log("Reconnected into room successfully.");
		RegisterRoomHandlers();
	}

	public void RegisterRoomHandlers()
	{
		m_SessionIdText = "sessionId: " + room.SessionId;
	
		room.State.entities.OnAdd += OnEntityAdd;
		room.State.entities.OnRemove += OnEntityRemove;
		room.State.TriggerAll();

		PlayerPrefs.SetString("roomId", room.Id);
		PlayerPrefs.SetString("sessionId", room.SessionId);
		PlayerPrefs.Save();

		room.OnLeave += (code) => Debug.Log("ROOM: ON LEAVE");
		room.OnError += (code, message) => Debug.LogError("ERROR, code =>" + code + ", message => " + message);
		room.OnStateChange += OnStateChangeHandler;

		room.OnMessage((Message message) =>
		{
			Debug.Log("Received Schema message:");
			Debug.Log(message.num + ", " + message.str);
		});

		room.OnMessage<MessageByEnum>((byte) MessageType.ONE, (message) =>
		{
			Debug.Log(">> Received message by enum/number => " + message.str);
		});

		room.OnMessage<TypeMessage>("type", (message) =>
		{
			Debug.Log("Received 'type' message!");
			Debug.Log(message);
		});

//		_ = room.Send((byte)MessageType.ONE, new MessageByEnum { str = "Sending message by enum/number" });
//		OnTankMove(); 
	}


	async void LeaveRoom()
	{
		await room.Leave(false);

		// Destroy player entities
		foreach (KeyValuePair<Entity, GameObject> entry in entities)
		{
			Destroy(entry.Value);
		}

		entities.Clear();
	}

	async void GetAvailableRooms()
	{
		var roomsAvailable = await client.GetAvailableRooms<CustomRoomAvailable>(roomName);

		Debug.Log("Available rooms (" + roomsAvailable.Length + ")");
		for (var i = 0; i < roomsAvailable.Length; i++)
		{
			Debug.Log("roomId: " + roomsAvailable[i].roomId);
			Debug.Log("maxClients: " + roomsAvailable[i].maxClients);
			Debug.Log("clients: " + roomsAvailable[i].clients);
			Debug.Log("metadata.str: " + roomsAvailable[i].metadata.str);
			Debug.Log("metadata.number: " + roomsAvailable[i].metadata.number);
		}
	}

	public void OnTankMove()
	{
//		Debug.Log("Sent Move to server");
		room.Send("move", new MoveData(){
		    xPos = myPlayer.transform.position.x, 
            yPos = 0.2f,
			zPos = myPlayer.transform.position.z,
			xRot = myPlayer.transform.rotation.x,
			yRot = myPlayer.transform.rotation.y,
			zRot = myPlayer.transform.rotation.z,
			wRot = myPlayer.transform.rotation.w
		});
	}


	void OnStateChangeHandler (State state, bool isFirstState )
	{
/*		entity.OnChange += (List<Colyseus.Schema.DataChange> changes) =>
		{
			myEnemy.transform.Translate(new Vector3(entity.xPos, entity.yPos, entity.zPos));
		};*/
//		Debug.Log("State has been updated!");
	}

	void OnEntityAdd(Entity entity, string key)


	{   
		Debug.Log("Entity Add ");
        var output = JsonUtility.ToJson(entity, true);
        Debug.Log(output);
		Debug.Log(entity.wRot);

		Debug.Log("249");
		Debug.Log("sessionID:" + entity.spawnpoint);

/*		switch (entity.spawnpoint)
		{
		case 1:
			myPlayer.transform.position = spawnPoint1.transform.position;
			myPlayer.transform.rotation = spawnPoint1.transform.rotation;
			break;
		case 2:
			myPlayer.transform.position = spawnPoint2.transform.position;
			myPlayer.transform.rotation = spawnPoint2.transform.rotation;
			break;
		case 3:
			myPlayer.transform.position = spawnPoint3.transform.position;
			myPlayer.transform.rotation = spawnPoint3.transform.rotation;
			break;
		case 4:
			myPlayer.transform.position = spawnPoint4.transform.position;
			myPlayer.transform.rotation = spawnPoint4.transform.rotation;
			break;
		case 5:
			myPlayer.transform.position = spawnPoint5.transform.position;
			myPlayer.transform.rotation = spawnPoint5.transform.rotation;
			break;
		case 6:
			myPlayer.transform.position = spawnPoint6.transform.position;
			myPlayer.transform.rotation = spawnPoint6.transform.rotation;
			break;
		case 7:
			myPlayer.transform.position = spawnPoint7.transform.position;
			myPlayer.transform.rotation = spawnPoint7.transform.rotation;
			break;
		case 8:
			myPlayer.transform.position = spawnPoint8.transform.position;
			myPlayer.transform.rotation = spawnPoint8.transform.rotation;
			break;
		case 9:
			myPlayer.transform.position = spawnPoint9.transform.position;
			myPlayer.transform.rotation = spawnPoint9.transform.rotation;
			break;
		case 10:
			myPlayer.transform.position = spawnPoint10.transform.position;
			myPlayer.transform.rotation = spawnPoint10.transform.rotation;
			break;
		case 11:
			myPlayer.transform.position = spawnPoint11.transform.position;
			myPlayer.transform.rotation = spawnPoint11.transform.rotation;
			break;
		case 12:
			myPlayer.transform.position = spawnPoint12.transform.position;
			myPlayer.transform.rotation = spawnPoint12.transform.rotation;
			break;
		case 13:
			myPlayer.transform.position = spawnPoint13.transform.position;
			myPlayer.transform.rotation = spawnPoint13.transform.rotation;
			break;
		case 14:
			myPlayer.transform.position = spawnPoint14.transform.position;
			myPlayer.transform.rotation = spawnPoint14.transform.rotation;
			break;
		case 15:
			myPlayer.transform.position = spawnPoint15.transform.position;
			myPlayer.transform.rotation = spawnPoint15.transform.rotation;
			break;
		case 16:
			myPlayer.transform.position = spawnPoint16.transform.position;
			myPlayer.transform.rotation = spawnPoint16.transform.rotation;
			break;
		case 17:
			myPlayer.transform.position = spawnPoint17.transform.position;
			myPlayer.transform.rotation = spawnPoint17.transform.rotation;
			break;
		case 18:
			myPlayer.transform.position = spawnPoint18.transform.position;
			myPlayer.transform.rotation = spawnPoint18.transform.rotation;
			break;
		}
/*


		/*
		Check for playerid before instantiating new enemy
		GameObject myEnemy = Instantiate(enemy, new Vector3(0,0,0), new Quaternion(0,0,0,0));

		Debug.Log("Enemy add! x => " + entity.xPos + ", z => " + entity.zPos);

		myEnemy.transform.position = new Vector3(entity.xPos, 0, entity.zPos);
		myEnemy.transform.rotation = new Quaternion(entity.xRot, entity.yRot, entity.zRot, entity.wRot);

		// Add "player" to map of players
		entities.Add(entity, myEnemy);
*/
	}

	void OnEntityRemove(Entity entity, string key)
	{
		GameObject cube;
		entities.TryGetValue(entity, out cube);
		Destroy(cube);

		entities.Remove(entity);
	}

	void OnApplicationQuit()
	{
	}
}
