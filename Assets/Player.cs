// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.16
// 

using Colyseus.Schema;

public partial class Player : Entity {
	[Type(9, "string")]
	public string sessionId = default(string);

	[Type(10, "boolean")]
	public bool connected = default(bool);
}

