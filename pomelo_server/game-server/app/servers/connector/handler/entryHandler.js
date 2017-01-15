

module.exports = function(app) {
	return new Handler(app);
};

var Handler = function(app) {
		this.app = app;
};

var handler = Handler.prototype;

/**
 * 直接根据用户名 进入到选择房间号，创建房间号的场景
 * 该方法返回已有的房间号
*/
handler.enterFirst = function(msg, session, next) {
    var self = this;
    var uid = msg.uid;
    var sessionService = self.app.get('sessionService'); // 每个用户都有一个session

    //duplicate log in
    if( !! sessionService.getByUid(uid)) {
        next(null, {
            code: 500,
            error: true
        });
        return;
    }

    session.bind(uid);
   // session.on('closed', onUserLeave.bind(null, self.app));

    //put user into channel
    self.app.rpc.chat.chatRemote.getRoomids(session, uid, self.app.get('serverId'), true, function(rooms){
        next(null, {
            rooms:rooms
        });
    });
};

/**
 * 自己创建房间号
 */
handler.enter = function(msg, session, next) {
	var self = this;
	var rid = msg.rid;
	var uid = msg.uid;
	var sessionService = self.app.get('sessionService'); // 每个用户都有一个session

	// 没有该用户 ，错误
	if( !sessionService.getByUid(uid)) {
		next(null, {
			code: 500,
			error: true
		});
		return;
	}

	session.set('rid', rid);
	session.push('rid', function(err) {
		if(err) {
			console.error('set rid for session service failed! error is : %j', err.stack);
		}
	});
	session.on('closed', onUserLeave.bind(null, self.app));

	//put user into channel
    self.app.rpc.chat.chatRemote.add(session, uid, self.app.get('serverId'), rid, true, function(rooms){
        next(null, {
            rooms:rooms
        });
    });
};

/**
 * 加入到一个已经存在的room中
 */
handler.enter2 = function(msg, session, next) {
    var self = this;
    var rid = msg.rid;
    var uid = msg.uid;
    var sessionService = self.app.get('sessionService'); // 每个用户都有一个session

    // 没有该用户 ，错误
    if( !sessionService.getByUid(uid)) {
        next(null, {
            code: 500,
            error: true
        });
        return;
    }

    session.set('rid', rid);
    session.push('rid', function(err) {
        if(err) {
            console.error('set rid for session service failed! error is : %j', err.stack);
        }
    });
    session.on('closed', onUserLeave.bind(null, self.app));

    //put user into channel
    self.app.rpc.chat.chatRemote.add2(session, uid, self.app.get('serverId'), rid, true, function(rooms){
        next(null, {
            rooms:rooms
        });
    });
};

/**
 * 用户直接退出了应用程序，需要删除这个用户
 */
var onUserLeave = function(app, session) {
	if(!session || !session.uid) {
		return;
	}
	app.rpc.chat.chatRemote.kick(session, session.uid, app.get('serverId'), session.get('rid'), null);
};