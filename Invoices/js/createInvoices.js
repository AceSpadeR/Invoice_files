var dataTable;


$(document).ready(function () {
    loadList();

});

function loadList() {
    var url = "/api/createInvoices";
    dataTable = $('#DT_CreateInvoices').DataTable({
        "buttons": [
            'excel',
            'pdf',
        ],

        "dom": "<'row'<'col-sm-6 py-1'lB><'col-sm-6 text-end py-1'f >>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-6'i><'col-sm-6'p>>",
        "ajax": {
            "url": url,
            "type": "GET",
            "datatype": "json",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.error(textStatus, errorThrown);
            }
        },
        "columns": [
            {
                "data": "id",
                "render": function (data) {
                    return '<input type="checkbox" class="invoice-checkbox" name="selectedOwnerships" value="' + data + '" aria-label="Ownership ' + data + '"  />';
                }, "width": "5%", "orderable": true, "searchable": false
            },
            
            { "data": "firstName", "width": "10%", "searchable": true },
            { "data": "lastName", "width": "10%", "searchable": true },
            { "data": "lotNumber", "width": "7%", "searchable": true },
            { "data": "email", "width": "7%", "searchable": true },
            { "data": "phoneNumber", "width": "12%", "searchable": false },
            { "data": "mobilePhoneNumber", "width": "12%", "searchable": false },
            {
                "data": "id",
                "render": function (data) {
                    var container = $('<div class="text-center"></div>');
                    var viewBtn = $('<a href="/Admin/Ownerships/Details?id=' + data + '" class="btn btn-info mx-1" style="cursor:pointer; "></a>');
                    viewBtn.append('<i class="bi bi-eye"></i>');
                    container.append(viewBtn);
                    return container.html();
                }, "width": "5%"
            }
        ],
        "language": {
            "emptyTable": "No data found."
        },
        "width": "100%",
        "order": [[3, "asc"]]
    });
    $('.dt-buttons').css('display', 'none');

}

