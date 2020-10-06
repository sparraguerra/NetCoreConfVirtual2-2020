const getFormatedDate = (date: Date): string => {
    const day = date.getDate();
    const month = date.getMonth() + 1;
    let dd = day.toString();
    let mm = month.toString();

    const yyyy = date.getFullYear();
    if (day < 10) {
        dd = '0'.concat(day.toString());
    }
    if (month < 10) {
        mm = '0'.concat(month.toString());
    }
    const formatedDate = dd + '/' + mm + '/' + yyyy;
    return formatedDate;
}

export const utils = {
    getFormatedDate
}

export const toastrOptions = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};