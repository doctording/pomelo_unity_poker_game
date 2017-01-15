var exp = module.exports;
/**
 * Module dependencies
 */
var pomelo = require('pomelo');
var Player = require('./player');

var rooms = {}

exp.getRooms = function () {
  return rooms;
}
/*
exp.getRoomIds = function () {
    var roomids = [];
    for(var i = 0; i < rooms.length; i++) {
        roomids[i] =  rooms.getName(i);
    }
    return roomids;
}

exp.getRoomUsersByRid = function (rid) {
    return rooms[rid];
}
*/
exp.createRoomByUser = function (user) {
  var room = {};
  room.createUser = user.uid;
  room.id = user.rid;

  // 为room添加用户
  room.users = {};
  room.users[user.uid] = user;
  room.number = 1;

  // 添加一个room
  rooms[room.id] = room;

  var channelService = pomelo.app.get('channelService');
  var channel = channelService.getChannel(room.id, true); // 创建一个channel
  channel.add(user.uid, user.sid); // uid,sid加入到channel中
}

/**
  * 删除某个房间，当此房间人数为零的时候，需要删除此房间
 */
exp.rmRoom = function(rid) {
    var e = rooms[rid];
    if (!e) {
        return false;
    }
    delete rooms[rid];
    return true;
}

/**
  * 新用户 加入到一个已经存在的房间中
 */
exp.addToARoom = function (user) {
    var e = rooms[user.rid];
    if (!e) {
        return false;
    }

    // 为room添加用户
    rooms[user.rid].users[user.uid] = user;
    rooms[user.rid].number =  rooms[user.rid].number + 1;  // 用户数量加上1

    var channelService = pomelo.app.get('channelService');
    var channel = channelService.getChannel( rooms[user.rid].id, true); // 创建或取得一个channel
    channel.add(user.uid, user.sid); // uid加入到channel中
}

/**
 * 删除房间中的某一个普通用户
 */
exp.rmCommonUserFromARoom = function (user) {
    var e = rooms[user.rid];
    if (!e) {
        return false;
    }

    // 删除该用户
    delete rooms[user.rid].users[user.uid];
    rooms[user.rid].number =  rooms[user.rid].number - 1;  // 用户数量减去1

    var channelService = pomelo.app.get('channelService');
    var channel = channelService.getChannel( rooms[user.rid].id, true); // 创建或取得一个channel
    channel.leave(user.uid, user.sid); // uid 从channel中删除
}

// 删除某一个房间
exp.rmRoomByRid = function(rid) {
    var e = rooms[rid];
    if (!e) {
        return false;
    }
    delete rooms[rid];
    return true;
}