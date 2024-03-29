﻿var message = {
    background: '', // 背景颜色
    outside: '', // 外框元素
    inside: '', // 信息显示元素
    insideSetTime: '', // 信息移除setTime
    body: '', // body元素
    time: 0, // 显示时间
    Ok(msg = "", time = 2000) {
        this.run(msg, 'success', time); 
    },
    Error(msg = "", time = 2000) {
        this.run(msg, 'error', time);
    },
    Info(msg = "", time = 2000) {
        this.run(msg, 'info', time);
    },
    Warn(msg = "", time = 2000) {
        this.run(msg, 'warning', time);
    },
    Toast(msg = "", time = 2000) {
        this.run(msg, '', time);
    },
    ToastCode(r) {
        if (r.code == 100)
            this.Ok(r.message);
        else this.Error(r.message);
    },
    run(msg = "success", type = 'success', time = 2000) {
        // 显示时间
        this.time = time;

        // 背景色
        this.background = this.backgroundCheck(type)

        // body
        this.body = document.body;

        // 时间戳id
        let id = 'inside_box' + Date.now();

        // 检查是否存在外框
        let outsideShow = document.getElementById('message_box_show');
        if (outsideShow != null) {
            // 文字显示区域
            this.inside = document.createElement('div');
            this.inside.setAttribute('class', 'message_box_inside cc-display')
            this.inside.setAttribute('id', id)
            this.inside.style.backgroundColor = this.background;
            this.inside.innerHTML = `<span>${msg}</span>`
            outsideShow.appendChild(this.inside);
        } else {
            // 最外框
            this.outside = document.createElement('div');
            this.outside.setAttribute('id', 'message_box_outside')
            this.outside.setAttribute('class', 'cc-display')

            // 中间区域
            outsideShow = document.createElement('div');
            outsideShow.setAttribute('id', 'message_box_show');

            // 文字显示区域
            this.inside = document.createElement('div');
            this.inside.setAttribute('class', 'message_box_inside cc-display')
            this.inside.setAttribute('id', id)
            this.inside.style.backgroundColor = this.background;
            this.inside.innerHTML = `<span>${msg}</span>`

            // 显示
            outsideShow.appendChild(this.inside);
            this.outside.appendChild(outsideShow);
            this.body.appendChild(this.outside);
        }

        // 添加监听
        this[id] = this.insideTime(this.inside, outsideShow);
        this.boxShowTime(this.inside, id, outsideShow);
    },

    // 信息显示区域展示
    boxShowTime(inside, insideSetTime, outsideShow) {
        inside.addEventListener('mouseleave', () => {
            // 离开后设置隐藏时间
            this[insideSetTime] = this.insideTime(inside, outsideShow);
        })
        inside.addEventListener('mouseenter', () => {
            // 清除隐藏设置
            clearTimeout(this[insideSetTime]);
        })
    },

    // 信息区显示
    insideTime(inside, outsideShow) {
        let insideSetTime = setTimeout(() => {
            outsideShow.removeChild(inside);
        }, this.time);
        return insideSetTime;
    },

    // 判定显示颜色
    backgroundCheck(type) {
        if (type === 'success') return '#67C23A';
        if (type === 'error') return '#F56C6C';
        if (type === 'warning') return '#E6A23C';
         if (type === 'info') return '#909399';
        return '#909399'; // 默认级别
    },
}

 