// 本地化时间参数
function j_format_time(f){
	if (f == ""){f = "en";}
	$(".format_date").each(function(i){
		$(this).text(_j_format_time_parse($(this).text(),'date',f));
	});
	
	$(".format_datetime").each(function(i){
		$(this).text(_j_format_time_parse($(this).text(),'datetime',f));
	});
}
// 将标准时间解析为参数
function _j_format_time_parse(tm,ty,f){
	var re = tm.match(/(\d+)-(\d+)-(\d+) (\d+):(\d+):(\d+)/);
	if (re == null){
		return tm;
	}else{
		if (f == 'en'){
			var month_map = new Array();
			month_map['01'] = 'Jan';
			month_map['02'] = 'Feb';
			month_map['03'] = 'Mar';
			month_map['04'] = 'Apr';
			month_map['05'] = 'May';
			month_map['06'] = 'Jun';
			month_map['07'] = 'Jul';
			month_map['08'] = 'Aug';
			month_map['09'] = 'Sep';
			month_map['10'] = 'Oct';
			month_map['11'] = 'Nov';
			month_map['12'] = 'Dec';
			
			
			if (ty == 'date'){
				return re[3] + '-' + month_map[re[2]] + '-' + re[1];
			}else if (ty == 'datetime'){
				return re[3] + '-' + month_map[re[2]] + '-' + re[1] + ' ' + re[4] + ':' + re[5] + ':' + re[6];
			}else if (ty == 'time'){
				return re[4] + ':' + re[5] + ':' + re[6];
			}
		}else if (f == 'zh'){
			
			if (ty == 'date'){
				return re[1] + '年' + re[2] + '月' + re[1] + '日';
			}else if (ty == 'datetime'){
				return re[1] + '年' + re[2] + '月' + re[1] + '日 ' + re[4] + ':' + re[5] + ':' + re[6];
			}else if (ty == 'time'){
				return re[4] + ':' + re[5] + ':' + re[6];
			}			
			
			
			
		}else if (f == 'es'){
			
			
			
			
			
		}		
	}
}

