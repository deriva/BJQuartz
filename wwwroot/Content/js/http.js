const _Authorization = 'token';
var _baseurl = '';
_baseurl = window.location.origin;
_baseurl = "/";
var cookike = document.cookie.split(';').find(x => x.indexOf('apiurl') > -1);
if (cookike)
    _baseurl = cookike.split('=')[1];


axios.defaults.timeout = 50000;
axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=UTF-8';
axios.defaults.headers['X-Requested-With'] = 'XMLHttpRequest';



axios.defaults.baseURL = _baseurl;
var http = {
    baseURL: () => {
        var cookike = document.cookie.split(';').find(x => x.indexOf('apiurl') > -1);
        if (cookike)
            _baseurl = cookike.split('=')[1];
        return _baseurl;
    },
    getToken: function () {
        return localStorage.getItem("token");
    },

    /*
      url
      params请求后台的参数,如：{name:123,values:['a','b','c']}
      loading是否显示遮罩层,可以传入true.false.及提示文本
      config配置信息,如{timeout:3000,headers:{token:123}}
    */
    post: function (url, params, config) {
        var layindex = 0;
        if (!config) { //默认不显示加载
            config = {};
            config.loading = true; config.check = true;//检测是否过期
        }
        if (config.loading) {
            layindex = layer.msg('正在努力加载中......', {
                icon: 16,
                shade: 0.21
            });
        }
        axios.defaults.headers["appcode"] = localStorage.getItem("appcode");
        //showLoading(loading);
        axios.defaults.headers[_Authorization] = http.getToken();
        return new Promise((resolve, reject) => {
            axios.post(url, params, config)
                .then(response => {
                    layer.close(layindex);
                    if (config.check) {
                        var flag = http.CheckIsExpirs(response.data);
                        if (flag) { return; }
                    }
                    var r = response.data;
                    if (r.code == 107) {//权限不足
                        top.layer.alert("权限不足,请前往首页,先获取证书：" + r.message, {
                            icon: 2,
                            shadeClose: false
                        }, function () {
                            top.layer.closeAll();
                        });
                        return;
                    }
                    resolve(r);
                }, err => {
                    layer.close(layindex);
                    reject(err && err.data && err.data.message ? err.data.message : '服务器处理异常');
                })
                .catch((error) => {
                    layer.close(layindex);
                    reject(error)
                })
        })
    },
    post2: function (path, params, config) {
        var url = "/Tool/GetData?path=BJ.Manage.Service/" + path
        return http.post(url, params, config); 
    },
    postasync: function (path, params, config) {
        var url = "/Tool/GetData2?path=BJ.Manage.Service/" + path;
        return http.post(url, params, config); 
    }, 
    //=true异步请求时会显示遮罩层,=字符串，异步请求时遮罩层显示当前字符串
    get: function (url, param, config) {
        //showLoading(loading); 

        var layindex = 0;
        if (param != null && param != "" && typeof param == "object") {
            if (url.indexOf("?") == -1) url += "?";
            url += LP.ParseParams(param);
        }
        if (!config) { //默认不显示加载
            config = {};
            config.loading = true;
            config.check = true;//检测是否过期
        }
        if (config.loading) {
            layindex = layer.msg('正在努力加载中......', {
                icon: 16,
                shade: 0.21
            });
        } axios.defaults.headers["appcode"] = localStorage.getItem("appcode");
        axios.defaults.headers[_Authorization] = http.getToken();
        return new Promise((resolve, reject) => {
            axios.get(url, param)
                .then(response => {
                    layer.close(layindex);
                    if (config.check) {
                        var flag = http.CheckIsExpirs(response.data);
                        if (flag) { return; }
                    }
                    var r = response.data;
                    if (r.code == 107) {//权限不足
                        top.layer.alert("权限不足,请前往首页,请先获取证书：" + r.message, {
                            icon: 2,
                            shadeClose: false
                        }, function () {
                            top.layer.closeAll();
                        }); return;
                    }
                    resolve(r);
                }, err => {
                    layer.close(layindex);
                    reject(err)
                })
                .catch((error) => {
                    layer.close(layindex);
                    reject(error)
                })
        })
    },
    CheckIsExpirs: (data) => {
        if (data.code == 102) {
            LP.ToastError("token信息已过期,请重新登录");
            setTimeout(() => { window.open("/account/vn/login", "login"); }, 800);
            return true;
        }
        return false;

    }

}

//exports('http', http);
