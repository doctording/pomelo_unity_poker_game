/**
 * Module dependencies
 */
var Room = require('../domain/entity/roomManager');

var exp = module.exports;

// global room container(roomId:roomObj)
var gRoomObjDict = {}; // 一个room所拥有的所有用户
var gRoomList = []; // 房间id列表
var gRoomNum = 0; // 有多少个房间

// create new room, add the player to the room
exp.createRoom = function(data, cb) {
  var roomObj = new Room(data.username, data.roomName);
  if(!!roomObj) {
    var result = roomObj.addPlayer(data, cb);
    if(result === consts.ROOM.JOIN_ROOM_RET_CODE.OK) {
      roomObj.setOwnerId(data.playerId);
      var realPlayerNum = data.realPlayerNum;
      if(!(realPlayerNum >= 1 && realPlayerNum <= consts.ROOM.MAX_MEMBER_NUM)) {
        realPlayerNum = 1;
      }
      roomObj.setRealPlayerNum(realPlayerNum);
      gRoomObjDict[roomObj.roomId] = roomObj;
      gRoomList.push(roomObj.roomId);
      ++gRoomNum;
      addSomeRobots2Room(roomObj, data);
      this.updateRoomList(roomObj.roomId);
    }
    return {result: result, roomId: roomObj.roomId};
  }
};

// testing code
exp.createRoom4Test = function(data, cb) {
  var roomObj = new Room(++gRoomId, data.roomName);
  if(!!roomObj) {
    var result = roomObj.addPlayer(data, cb);
    if(result === consts.ROOM.JOIN_ROOM_RET_CODE.OK) {
      roomObj.setOwnerId(data.playerId);
      var realPlayerNum = data.realPlayerNum;
      if(!(realPlayerNum >= 1 && realPlayerNum <= consts.ROOM.MAX_MEMBER_NUM)) {
        realPlayerNum = 1;
      }
      roomObj.setRealPlayerNum(realPlayerNum);
      gRoomObjDict[roomObj.roomId] = roomObj;
      gRoomList.push(roomObj.roomId);
      ++gRoomNum;
      addSomeRobots2Room(roomObj, data);
    }
    return {result: result, roomId: roomObj.roomId};
  }
};

exp.createSomeRooms = function() {
  for(var i = 1; i <= 30; i++) {
    var playerId = consts.ROOM.MAX_RANDOM_NUM - i;
    var playerNum = Math.floor(Math.random() * 5 + 2)
    var data = {
      playerId: playerId,
      playerName: 'RoomMaker_' + i,
      realPlayerNum: playerNum,
      roomName: 'RoomName_' + i,
      serverId: 'game-server-1'
    };
    this.createRoom4Test(data);
  }
};
// testing code

exp.enterRoom = function(msg, cb) {
  var data = {
    playerId: parseInt(msg.playerId),
    playerName: msg.playerName,
    realPlayerNum: parseInt(msg.realPlayerNum),
    roomName: msg.roomName,
    serverId: msg.serverId
  };
  // utils.myPrint('enterRoom : data = ', JSON.stringify(data));

  var roomObj = gRoomObjDict[msg.roomId];
  var result = consts.ROOM.FAILED;
  if(!roomObj) {
    result = this.createRoom(data, cb);
  } else {
    if(!roomObj.isRoomFull()) {
      result = roomObj.addPlayer(data, cb);
      if(result === consts.ROOM.JOIN_ROOM_RET_CODE.OK) {
        // try to start round
        if(roomObj.isRoomFull()) {
          roomObj.startRound();
        } else {
          this.updateRoomList(roomObj.roomId);
        }
      }
    } else {
      utils.invokeCallback(cb, {seatNum: -1});
    }
  }

  // utils.myPrint('result = ', JSON.stringify(result));
  return result;
};

exp.kick = function(data) {
  var roomObj = gRoomObjDict[data.roomId];
  if(!roomObj) {
    return;
  }
  var ret = roomObj.kick(data);
  if(ret) {
    var isEmpty = roomObj.getIsEmpty();
    // utils.myPrint('isEmpty = ', isEmpty);
    if(isEmpty) {
      roomObj.clearAllTimers();
      this.removeRoomById(data.roomId);
    }
    return this.getRoomList({pageNum: 1});
  }
};

exp.getRoomById = function(roomId) {
  var roomObj = gRoomObjDict[roomId];
  return roomObj || null;
};

exp.selectRoleId = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  var ret = roomObj.selectRoleId(msg.playerId, msg.roleId);
  return ret;
};

exp.roundEnd = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  var isPass = roomObj.checkCurRoleId(msg);
  // utils.myPrint('isPass = ', isPass);
  if(!isPass) {
    return;
  }
  // all roles come on stage one by one
  // console.log('msg = ', JSON.stringify(msg));
  var playerId = parseInt(msg.playerId);
  roomObj.triggerAllEffect(consts.ROOM.EFFECT_TRIGGER_TIMING.ROUND_END, playerId);
  var curRoleId = parseInt(msg.curRoleId) + 1;
  if(curRoleId > consts.ROOM.ROLE_ID.QUEEN) {
    roomObj.allRoleRoundEnd(curRoleId);
  } else {
    roomObj.comeOnStage1by1(curRoleId);
  }
  return true;
};

// construct building
exp.constructBuilding = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.constructBuilding(msg);
};

// select coin
exp.selectCoin = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.selectCoin(msg);
};

// select blueprint
exp.selectBlueprint = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.selectBlueprint(msg);
};

// get kill or steal id list
exp.getKillOrStealIdList = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.getKillOrStealIdList(msg);
};

// kill a role
exp.kill = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.kill(msg);
};

// steal a role
exp.steal = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.steal(msg);
};

// exchange cards with sys
exp.exchangeCardWithSys = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.exchangeCardWithSys(msg);
};

// exchange cards with player
exp.exchangeCardWithPlayer = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.exchangeCardWithPlayer(msg);
};

// dismantle building
exp.dismantleBuilding = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.dismantleBuilding(msg);
};

// collect taxes
exp.collectTaxes = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.collectTaxes(msg);
};


// do action for robot
exp.doAction4robot = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return;
  }
  return roomObj.doAction4robot(msg);
};

exp.removeRoomById = function(roomId) {
  var roomObj = gRoomObjDict[roomId];
  if(!roomObj) {
    return {result: consts.ROOM.FAILED};
  }

  delete gRoomObjDict[roomId];

  var idx = gRoomList.indexOf(roomId);
  if(idx > -1) {
    gRoomList.splice(idx, 1);
    gRoomNum = Math.max(gRoomNum - 1, 0);
  }
};

exp.getRoomListContent = function(roomId) {
  var roomObj = gRoomObjDict[roomId];
  if(!roomObj) {
    return;
  }

  var idx = gRoomList.indexOf(roomId);
  if(idx === -1) {
    return;
  }

  var maxPageNum = Math.ceil(gRoomNum / consts.ROOM.ROOM_NUM_PER_PAGE);
  var curPageNum = Math.ceil((idx+1) / consts.ROOM.ROOM_NUM_PER_PAGE);
  var roomInfoList = [];
  var startIdx = Math.max((curPageNum - 1) * consts.ROOM.ROOM_NUM_PER_PAGE, 0);
  var endIdx = curPageNum * consts.ROOM.ROOM_NUM_PER_PAGE;
  // utils.myPrint('startIdx, endIdx = ', startIdx, endIdx);
  for(var i = startIdx; i < endIdx; i++) {
    var rId = gRoomList[i];
    var rO = gRoomObjDict[rId];
    if(!rO) {
      continue;
    }
    var playerNum = rO.getPlayerNum();
    playerNum = playerNum + ' / ' + consts.ROOM.MAX_MEMBER_NUM;
    var isFull = rO.isRoomFull();
    var d = {roomId: rO.roomId, roomName: rO.name, playerNum: playerNum, isFull: isFull};
    roomInfoList.push(d);
  }

  return {curPageNum: curPageNum, maxPageNum: maxPageNum, roomInfoList: roomInfoList};
};

exp.updateRoomList = function(roomId) {
  var roomObj = gRoomObjDict[roomId];
  if(!roomObj) {
    return;
  }

  var infoDict = this.getRoomListContent(roomId);
  if(!!infoDict) {
    roomObj.channel.pushMessage('onUpdateRoomList', infoDict, null);
  }
};

exp.getRoomList = function(msg) {
  if(gRoomList.length === 0) {
    return {};
  }

  var pageNum = parseInt(msg.pageNum);

  var startIdx = Math.max((pageNum - 1) * consts.ROOM.ROOM_NUM_PER_PAGE, 0);
  var rId = gRoomList[startIdx];
  var rO = gRoomObjDict[rId];
  if(!rO) {
    return {};
  }

  return this.getRoomListContent(rId);
};

exp.chatInRoom = function(msg) {
  var roomObj = gRoomObjDict[msg.roomId];
  if(!roomObj) {
    return false;
  }

  return roomObj.pushChatMsg2All(msg);
};

