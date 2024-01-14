//业务组件
var BusComponent = {
    AppCode: (cb) => { 
        $("[name=appcode]").empty();
        $("[name=appcode]").append("<option value=''> 请选择</option>")
        http.get("/APPParmater/GetProductDataList").then(r => {
            if (r.code == 100) {
                var arr = r.Items;
                arr.forEach(function (item, index) {
                    $("[name=appcode]").append("<option value='" + item.appcode + "'>" + item
                        .appname + "</option>")
                });
                if (cb) cb();
            }
        });

    }
}

 
 