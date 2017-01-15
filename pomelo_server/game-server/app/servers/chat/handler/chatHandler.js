var chatRemote = require('../remote/chatRemote');

var player = require('../../../domain/entity/player');
var roomManager = require('../../../domain/entity/roomManager');
var cards = require('../../../domain/entity/cards');

module.exports = function(app) {
	return new Handler(app);
};

var Handler = function(app) {
	this.app = app;
};

var handler = Handler.prototype;

/**
 * Send messages to users
 *
 * @param {Object} msg message from client
 * @param {Object} session
 * @param  {Function} next next stemp callback
 *
 */
handler.send = function(msg, session, next) {
	var uid = msg.from;
	var rid = session.get('rid');
	var channelService = this.app.get('channelService');
	var channel = channelService.getChannel(rid, false);

    var param = {
        msg: msg.content,
        from: uid,
    };

	channel.pushMessage('onChat', param);

	next(null, {
		route: msg.route
	});
};

/**
 * 是否准备好了
 *
 */
handler.isready = function(msg, session, next) {
    var uid = msg.uid;
    var rid = msg.rid;
    var index = msg.index;

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

    var arrayPuke = cards.getRandomCards();

    var channelService = this.app.get('channelService');
    channelService.pushMessageByUids('onReady',{uid:uid,index:index},uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    next(null, {
        route: arrayPuke
    });
}

handler.fapai = function(msg, session, next) {
    var uid = msg.uid;
    var rid = msg.rid;
    var index = msg.index;

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

	 var arrayPuke = cards.getRandomCards();
     var channelService = this.app.get('channelService');
	 channelService.pushMessageByUids('onStartGame',{arrayPuke:arrayPuke },uidArrayRid,function(err){
		 if(err){
			 console.log(err);
			 return;
		 }
	 });

    next(null, {
        route: arrayPuke
    });
};

/**
 *  某个用户出牌，需要广播这个消息给其它的玩家
 *
 */
handler.chupai = function(msg, session, next) {
    var uid = msg.uid;
    var rid = msg.rid;
    var index = msg.index;
    var deleteCards = msg.delete_cards;

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

    var channelService = this.app.get('channelService');
    channelService.pushMessageByUids('onChupai',{uid:uid, rid:rid, index:index, deleteCards:deleteCards}, uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    next(null, {
        uid:uid,
        rid:rid,
        index:index,
        deleteCards:deleteCards
    });
};

/**
 *  某个用户不出牌
 *
 */
handler.buchupai = function(msg, session, next) {
    var uid = msg.uid;
    var rid = msg.rid;
    var index = msg.index;

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

    var channelService = this.app.get('channelService');
    channelService.pushMessageByUids('onBuchupai',{uid:uid, rid:rid, index:index}, uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    next(null, {
        uid:uid,
        rid:rid,
        index:index,
    });
};


handler.over = function(msg, session, next) {
    var uid = msg.uid;
    var rid = msg.rid;
    var index = msg.index;

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

    var channelService = this.app.get('channelService');
    channelService.pushMessageByUids('onOver',{uid:uid, rid:rid, index:index}, uidArrayRid,function(err){
        if(err){
            console.log(err);
            return;
        }
    });

    next(null, {
        uid:uid,
        rid:rid,
        index:index
    });
};