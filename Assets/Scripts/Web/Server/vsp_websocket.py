from fastapi import FastAPI
from starlette.websockets import WebSocket
from fastapi.middleware.cors import CORSMiddleware

import json
import time
import uvicorn
import sys
import asyncio
import threading

app = FastAPI()
app.add_middleware(
	CORSMiddleware,
	allow_origins=["*"],
	allow_credentials=True,
	allow_methods=["*"],
	allow_headers=["*"],
)


class Player:
	def __init__(self, player_name, player_id):
		self.position = [0, 0, 0]
		self.rotation = [0]

		self.player_name = player_name
		self.player_id = player_id

#class that is turned into a json and sent to client
class ClientInfo:
	def __init__(self, players, recent_messages):
		self.players = players
		self.recent_messages = recent_messages

	#https://stackoverflow.com/questions/3768895/how-to-make-a-class-json-serializable
	def to_JSON(self):
		return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True, indent=4)

players = {} #dictionary with all players; id is key and class is value

@app.websocket("/server/{player_name}")
async def server_websocket (websocket: WebSocket, player_name: str):
	global players
	player_id = -1 #id is assigned after websocket is connected
	#try:
	await websocket.accept()

	while True:
		#data is sent by client
		data = await asyncio.wait_for(websocket.receive_text(), 10) #10s timeout; when timed error is thrown

		return_text = "error"

		#if data is still a string requesting for id return an id back
		if data == "request_id":
			if player_id == -1:
				player_id = assign_player_id()
				#create new player
				players[player_id] = Player(player_name=player_name, player_id=player_id)
			return_text = str(player_id)
		else:
			#data should be a json file that can be parsed
			try:
				player_info = json.loads(data)
				players[player_info["pid"]].position = [player_info["position_x"], player_info["position_y"], player_info["position_z"]]
				players[player_info["pid"]].rotation = [player_info["rotation_y"]]

				#add rotation, new messages, etc
			except:
				print("player info json could not be parsed")

			#return a json string of all player classes
			return_text = ClientInfo(players=players, recent_messages=[]).to_JSON()

		#return data to client side
		await asyncio.wait_for(websocket.send_text(return_text), 10)
	# except:
	# 	#remove player coordinate from positions
	# 	if player_id != -1:
	# 		del players[player_id]

	# 	print("player #" + str(player_id) + " has left the game")

def assign_player_id():
	global players
	#if id is taken, move up by 1
	i = 0
	while i in players.keys():
		i += 1
	return i

uvicorn.run(app, port=8000, host="0.0.0.0")
