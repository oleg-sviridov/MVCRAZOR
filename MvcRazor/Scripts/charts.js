
$(document).ready(function () {
    $('.container').on('click', 'input[type="button"]', function () {
        $.ajax({
            //запрашиваемся в действие Draw контроллера и принимаем json list
            url: 'Product/Draw',
            dataType: "json",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            async: false,
            processData: false,
            cache: false,
            delay: 200000,
            //если результат запроса успешен, то выбираем список 10 самых частых значений в массив
            success: function (data) {
                var series = new Array();
                for (var i in data) {
                    var serie = new Array(data[i].Item, data[i].Value);
                    series.push(serie);
                }
                //вызываем функцию рисования графика
                DrawChart(series);
            },
            error: function (xhr) {
                alert("Произошла ошибка чтения файла, возможно, данные были изменены");
            }
        });
    })
});


function DrawChart(series) {
    var chart;

    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'chart1',
            plotBackgroundColor: null,
            plotBorderWidth: 1, //null,
            plotShadow: false
        },
        title: {
            text: 'График из последовательности 10 самых повторяющихся значений'
        },
        xAxis: {
            title: {
                text: 'Позиция'
            }
        },
        yAxis: {
            title: {
                text: 'Значение'
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },
        series: [{
            name: 'Случайные числа',
            data: series
        }]
    });
};