var player = require('../../../domain/entity/player');
var roomManager = require('../../../domain/entity/roomManager');
var cards = require('../../../domain/entity/cards');

module.exports = function(app) {
	return new ChatRemote(app);
};

var ChatRemote = function(app) {
	this.app = app;
	this.channelService = app.get('channelService');
};


ChatRemote.prototype.getRoomids = function(uid, sid,flag, cb) {
    var user = {};
    user.uid = uid;
    user.sid = sid;
    player.addUser(user);

	cb(
		roomManager.getRooms()
    );
};

/**
 * 创建一个新的房间
 */
ChatRemote.prototype.add = function(uid, sid, rid, flag, cb) {
	var user = {};
	user.uid = uid;
	user.rid = rid;
	user.sid = sid;
    user.index = 1;

	//player.addUser(user);
    roomManager.createRoomByUser(user);

    // 需要向 所有 未进入真正游戏的用户 推送一条消息：有一个新房间被创建出来了
	// TODO
	/*
	var urs =  roomManager.getRooms();
    for( var pro in urs)
    {
        var channel = this.channelService.getChannel(pro, false);
        var param = {
            route: 'onAddRoom',
            rid: pro
        };
        channel.pushMessage(param); // push message to client(should use onAdd to receive the message)
    }
	*/
	var uidArray = new Array();
	var urs = player.getUsers();
	for(var pro in urs)
	{
		var uidObject = {};
		uidObject.uid = urs[pro].uid;
		uidObject.sid = urs[pro].sid;
		uidArray.push(uidObject);
	}

    this.channelService.pushMessageByUids('onAddRoom',{rid:rid,num:1},uidArray,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    /**/
	cb(
		// player.getUsers()
		roomManager.getRooms()
    );
};

ChatRemote.prototype.add2 = function(uid, sid, rid, flag, cb) {
    var user = {};
    user.uid = uid;
    user.rid = rid;
    user.sid = sid;


    var roomsT =  roomManager.getRooms();
    var numCnt = roomsT[rid].number;

    user.index = numCnt + 1;

    player.addUser(user);
    roomManager.addToARoom(user); // 添加用户，房间人数增加了

    // 这里需要判断该用户加入的房间的人数是否已经满足要求了，如果是，需要广播消息？？
	// TODO
    var uidArray = new Array(); // 所有的用户
    var urs = player.getUsers();
    for(var pro in urs) {
        var uidObject = {};
        uidObject.uid = urs[pro].uid;
        uidObject.sid = urs[pro].sid;
        uidArray.push(uidObject);
    }

	this.channelService.pushMessageByUids('onAddCommonUser',{rid:rid,num: user.index },uidArray,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    // 如果用户是房间中最后一个用户，那么需要给房间中的人发送 游戏开始，并且初始游戏场景等等 ??
    // TODO
	// 普通用户加入游戏，只给同房间的人发送消息
    var uidArrayRid = new Array() ; // 当前房间的用户列表
    var roomsAll = roomManager.getRooms();
    var userAll =  roomsAll[rid].users;
    for(var pro2 in userAll)
    {
        var uidObject2 = {};
        uidObject2.uid = userAll[pro2].uid;
        uidObject2.sid = userAll[pro2].sid;
        uidArrayRid.push(uidObject2);
    }
	this.channelService.pushMessageByUids('onCommonUserAddToGame',{rid:rid,uid:uid,index:user.index },uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

/*
    // 房间的人数达到要求了
    if(user.index == 3)
    {
        var arrayPuke = cards.getRandomCards();
        this.channelService.pushMessageByUids('onStartGame',{arrayPuke:arrayPuke },uidArrayRid,function(err){
            if(err){
                console.log(err);
                return;
            }
        });
    }
*/
    cb(
        //player.getUsers()
        roomManager.getRooms()
    );
};

/**
 * Get user from chat channel.
 *
 * @param {Object} opts parameters for request
 * @param {String} name channel name
 * @param {boolean} flag channel parameter
 * @return {Array} users uids in channel
 *
 */
ChatRemote.prototype.get = function(name, flag) {
	var users = [];
	var channel = this.channelService.getChannel(name, flag);
	if( !! channel) {
		users = channel.getMembers();
	}
	for(var i = 0; i < users.length; i++) {
		users[i] = users[i].split('*')[0];
	}
	return users;
};

/**
 * Kick user out chat channel.
 *
 * @param {String} uid unique id for user
 * @param {String} sid server id
 * @param {String} name channel name
 *
 */
ChatRemote.prototype.kick = function(uid, sid, rid) {
	/*
	var channel = this.channelService.getChannel(name, false);
	// leave channel
	if( !! channel) {
		channel.leave(uid, sid);
	}
	var username = uid.split('*')[0];
	var param = {
		route: 'onLeave',
		user: username
	};
	channel.pushMessage(param);
	*/

	var u = {uid:uid,sid:sid,rid:rid};
	roomManager.rmCommonUserFromARoom(u);
	player.rmUser(uid);

    var uidArray = new Array();
    var urs = player.getUsers();
    for(var pro in urs)
    {
        var uidObject = {};
        uidObject.uid = urs[pro].uid;
        uidObject.sid = urs[pro].sid;
        uidArray.push(uidObject);
    }

    var roomsT =  roomManager.getRooms();
    var numCnt = roomsT[rid].number;

    if(numCnt == 0) // 某个房间没有人了，该房间就可以删除了
        roomManager.rmRoomByRid(rid);

    this.channelService.pushMessageByUids('onDelCommonUser',{rid:rid,num:numCnt},uidArray,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    // 向该房间的其它用户发送退出的消息
    // TODO

    var uidArrayRid = new Array() ; // 当前房间的用户列表
    var roomsAll = roomManager.getRooms();
    var userAll =  roomsAll[rid].users;
    for(var pro2 in userAll)
    {
        var uidObject2 = {};
        uidObject2.uid = userAll[pro2].uid;
        uidObject2.sid = userAll[pro2].sid;
        uidArrayRid.push(uidObject2);
    }
    this.channelService.pushMessageByUids('onDelCommonUserInRoom',{rid:rid,uid:uid},uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

/*
    var uidArray = new Array(); // 所有的用户
    var urs = player.getUsers();
    for(var pro in urs)
    {
        var uidObject = {};
        uidObject.uid = urs[pro].uid;
        uidObject.sid = urs[pro].sid;
        uidArray.push(uidObject);
    }

    var roomsT =  roomManager.getRooms();
    var numCnt = roomsT[rid].number;
    this.channelService.pushMessageByUids('onDelCommonUserInRoom',{uid:uid,rid:rid,num:numCnt},uidArray,function(err){
        if(err){
            console.log(err);
            return;
        }
    });
*/
};