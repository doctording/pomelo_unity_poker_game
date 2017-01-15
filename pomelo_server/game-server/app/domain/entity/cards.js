var exp = module.exports;

var carUser = {}; // 3个用户的牌

function randomSort(arr, newArr) {
    // 如果原数组arr的length值等于1时，原数组只有一个值，其键值为0
    // 同时将这个值push到新数组newArr中
    if(arr.length == 1) {
        newArr.push(arr[0]);
        return newArr; // 相当于递归退出
    }

    // 在原数组length基础上取出一个随机数
    var random = Math.ceil(Math.random() * arr.length) - 1;
    // 将原数组中的随机一个值push到新数组newArr中
    newArr.push(arr[random]);
    // 对应删除原数组arr的对应数组项
    arr.splice(random,1);
    return randomSort(arr, newArr);
}

exp.getRandomArray = function() {

    var arr = [];
    var index = 0;
    for(var i = 101;i<=113;i++)
        arr[index++] = i;
    for(var i = 201;i<=213;i++)
        arr[index++] = i;
    for(var i = 301;i<=313;i++)
        arr[index++] = i;
    for(var i = 401;i<=413;i++)
        arr[index++] = i;

    var newArr = [];
    randomSort(arr,newArr);

    return newArr;
}

exp.initCards = function() {
    var arr = this.getRandomArray();

    var u1 = [];
    for(var i=0;i<17;i++){
        u1[i] = arr[i];
    }

    var u2 = [];
    for(var i=17;i<34;i++){
        u2[i-17] = arr[i];
    }

    var u3 = [];
    for(var i=34;i<51;i++){
        u3[i-34] = arr[i];
    }

    carUser.u1 = u1;
    carUser.u2 = u2;
    carUser.u3 = u3;
}

exp.getRandomCards = function() {
    this.initCards();
    return carUser;
}