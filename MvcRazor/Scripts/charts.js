
$(function () {
    $.ajax({
        url: 'Home/VehicleSummary',
        dataType: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        async: false,
        processData: false,
        cache: false,
        delay: 20000,
        success: function (data) {

            var series = new Array();

            for (var i in data) {
                var serie = new Array(data[i].Item, data[i].Value);
                series.push(serie);
            }
            DrawPieChart(series);
        },
        error: function (xhr) {
            alert("error");
        }
    });
});


function DrawPieChart(series) {
    var chart;

    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'chart1',
            plotBackgroundColor: null,
            plotBorderWidth: 1, //null,
            plotShadow: false
        },
        title: {
            text: 'Solar Employment Growth by Sector, 2010-2016'
        },

        subtitle: {
            text: 'Source: thesolarfoundation.com'
        },

        yAxis: {
            title: {
                text: 'Number of Employees'
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },

        series: [{
            name: 'Installation',
            data: series
        }]
    });
};