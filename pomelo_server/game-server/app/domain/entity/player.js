var exp = module.exports;

var users = {};

exp.getUsers = function() {
    return users;
}

exp.addUser = function(e) {
    if (!e || !e.uid) {
        return false;
    }
    if(users[e.uid])
        return false;
    else {
        users[e.uid] = e;
        return true;
    }
}

exp.rmUser = function(uid) {
    var e = users[uid];
    if (!e) {
        return false;
    }
    delete users[uid];
    return true;
}

exp.updateUser = function(e) {
    if (!e || !e.uid) {
        return false;
    }
    if(users[e.uid])
    {
        users[e.uid] = e;
        return true;
    }else
        return false;
}