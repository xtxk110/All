/*依赖于jquery
*/
(function ($) {
    $.alerts = {
        alert: function (title, message, callback) {
            if (title == null) title = 'Alert';
            $.alerts._show(title, message, null, 'alert', function (result) {
                if (callback) callback(result);
            });
        },

        confirm: function (title, message, callback) {
            if (title == null) title = 'Confirm';
            $.alerts._show(title, message, null, 'confirm', function (result) {
                if (callback) callback(result);
            });
        },


        _show: function (title, msg, value, type, callback) {

            var _html = "";

            _html += '<div id="mb_box"></div><div id="mb_con"><span id="mb_tit">' + title + '</span>';
            _html += '<div id="mb_msg">' + msg + '</div><div id="mb_btnbox">';
            if (type == "alert") {
                _html += '<input id="mb_btn_ok" type="button" value="确定" />';
            }
            if (type == "confirm") {
                _html += '<input id="mb_btn_ok" type="button" value="确定" />';
                _html += '<input id="mb_btn_no" type="button" value="取消" />';
            }
            _html += '</div></div>';

            //必须先将_html添加到body，再设置Css样式  
            $("body").append(_html); GenerateCss();

            switch (type) {
                case 'alert':

                    $("#mb_btn_ok").click(function () {
                        $.alerts._hide();
                        if (callback) callback(true);
                    });
                    $("#mb_btn_ok").focus().keypress(function (e) {
                        if (e.keyCode == 13 || e.keyCode == 27) $("#mb_btn_ok").trigger('click');
                    });
                    break;
                case 'confirm':

                    $("#mb_btn_ok").click(function () {
                        $.alerts._hide();
                        if (callback) callback(true);
                    });
                    $("#mb_btn_no").click(function () {
                        $.alerts._hide();
                        if (callback) callback(false);
                    });
                    $("#mb_btn_no").focus();
                    $("#mb_btn_ok, #mb_btn_no").keypress(function (e) {
                        if (e.keyCode == 13) $("#mb_btn_ok").trigger('click');
                        if (e.keyCode == 27) $("#mb_btn_no").trigger('click');
                    });
                    break;
            }
        },
        _hide: function () {
            $("#mb_box,#mb_con").remove();
        }
    }
    // Shortuct functions  
    myAlert = function (title, message, callback) {
        $.alerts.alert(title, message, callback);
        $("#mb_box").on("touchmove", function (event) {
            event.preventDefault();
        }); 
    }

    myConfirm = function (title, message, callback) {
        $.alerts.confirm(title, message, callback);
        $("#mb_box").on("touchmove", function (event) {
            event.preventDefault();
        }); 
    };



    //生成Css  
    var GenerateCss = function () {

        $("#mb_box").css({
            width: '100%', height: '100%', zIndex: '99999', position: 'fixed',
            filter: 'Alpha(opacity=60)', backgroundColor: 'black', top: '0', left: '0', opacity: '0.6'
        });

        $("#mb_con").css({
            zIndex: '999999', width: '260px', height: '210px', position: 'absolute',
            backgroundColor: 'White', borderRadius: '5px', top: '50%', left: '50%', marginTop: '-100px', marginLeft: '-125px'
        });

        $("#mb_tit").css({
            display: 'block', fontSize: '14px', color: '#444', padding: '0 15px',
            backgroundColor: '#fff', height: '30px', lineHeight: '30px', borderBottom:'1px solid #e4e4e4',
            fontWeight: 'bold'
        });

        $("#mb_msg").css({
            padding: '10px', lineHeight: '20px', textAlign: 'center', fontSize: '16px', color: '#4c4c4c',
            height: '100px', overFlow: 'hidden', display: 'flex', justifyContent: 'center', alignItems: 'center'
        });


        $("#mb_btnbox").css({ marginTop: '10px', textAlign: 'center' });
        $("#mb_btn_ok,#mb_btn_no").css({ width: '80px', height: '40px', color: 'white', border: 'none', borderRadius: '4px', fontSize: '16px' });
        $("#mb_btn_ok,#mb_btn_no").css("-webkit-appearance", "none").css("-webkit-tap-highlight-color", "#000");
        $("#mb_btn_no").css({ backgroundColor: '#6495ED' });
        $("#mb_btn_ok").css({ backgroundColor: '#6495ED', marginRight: '40px' });
    }
    
})(jQuery);