var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    var url = "/api/invoices";
    dataTable = $('#DT_Invoices').DataTable({
        "deferRender": true,
        "buttons": ['excel', 'pdf'],
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
                    var container = $('<div class="text-center"></div>');
                    var viewBtn = $('<a href="/Admin/Invoices/Details?id=' + data + '" class="btn btn-info mx-1" style="cursor:pointer;"></a>');
                    viewBtn.append('<i class="bi bi-eye"></i>');
                    container.append(viewBtn);
                    return container.html();
                },
                "width": "5%" 
            },
            { "data": "invoiceNumber", "searchable": true, "width": "10%" },
            { "data": "invoiceTitle", "searchable": true, "width": "15%" },
            { "data": "ownerFirstName", "searchable": true, "width": "10%" },
            { "data": "ownerLastName", "searchable": true, "width": "10%" },
            {
                "data": "invoiceAmount",
                "searchable": false,
                "render": function (data) {
                    // US dollars
                    return data.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
                },
                "width": "10%"
            },
            {
                "data": "invoiceDate",
                "searchable": false,
                "render": function (data) {
                    // MM/DD/YYYY
                    return new Date(data).toLocaleDateString('en-US');
                },
                "width": "10%"
            },
            {
                "data": "invoiceDueDate",
                "searchable": false,
                "render": function (data) {
                    // MM/DD/YYYY
                    return new Date(data).toLocaleDateString('en-US');
                },
                "width": "10%"
            },
            {
                "data": "invoicePaid",
                "searchable": true,
                "render": function (data) {
                    if (data) {
                        return '<span class="badge bg-success">Paid</span>';
                    } else {
                        return '<span class="badge bg-danger">Unpaid</span>';
                    }
                },
                "width": "10%"
            },
            { "data": "invoiceNotes", "searchable": false, "width": "10%" }
        ],
        "language": {
            "emptyTable": "No data found."
        },
        "width": "100%",
        "order": [[6, "desc"]],
        "initComplete": function () {
           

            // Add date invoiceDate column
            var invoiceDateColumn = this.api().column(6);
            $('<input type="date" class="form-control" placeholder="Search..." />')
                .appendTo($(invoiceDateColumn.footer()).empty())
                .on('change', function () {
                    var dateText = this.value;
                    if (dateText) {
                        var date = new Date(dateText);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var formattedDate = month + '/' + day + '/' + date.getFullYear();
                        invoiceDateColumn.search(formattedDate).draw();
                    } else {
                        invoiceDateColumn.search('').draw();
                    }
                });

            // Add date invoiceDueDate column
            var invoiceDueDateColumn = this.api().column(7);
            $('<input type="date" class="form-control" placeholder="Search..." />')
                .appendTo($(invoiceDueDateColumn.footer()).empty())
                .on('change', function () {
                    var dateText = this.value;
                    if (dateText) {
                        var date = new Date(dateText);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var formattedDate = month + '/' + day + '/' + date.getFullYear();
                        invoiceDueDateColumn.search(formattedDate).draw();
                    } else {
                        invoiceDueDateColumn.search('').draw();
                    }
                });

            // Add filter invoicePaid
            var invoicePaidColumn = this.api().column(8);
            var select = $('<select><option value="">All</option></select>')
                .appendTo($(invoicePaidColumn.footer()))
                .on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex($(this).val());
                    invoicePaidColumn.search(val ? '^' + val + '$' : '', true, false).draw();
                });
            select.append('<option value="Paid">Paid</option>');
            select.append('<option value="Unpaid">Unpaid</option>');

        }


    });
    $('.dt-buttons').css('display', 'none');
}
