// trả ngày về chuỗi yyyy-MM-dd
function formatDateYMD(currentDate) {
    let year = currentDate.getFullYear();
    let month = String(currentDate.getMonth() + 1).padStart(2, '0');
    let day = String(currentDate.getDate()).padStart(2, '0');
    let result = `${year}-${month}-${day}`;
    return result;
}

// trả ngày về chuỗi dd-MM-yyyy
function formatDateDMY(currentDate) {
    let year = currentDate.getFullYear();
    let month = String(currentDate.getMonth() + 1).padStart(2, '0');
    let day = String(currentDate.getDate()).padStart(2, '0');
    let result = `${day}-${month}-${year}`;
    return result;
}
// trả về chuỗi yyyy-MM-dd với input là chuỗi
function formatStrYMD(strInput) {
    let ngay = strInput.split('-')[0];
    let thang = strInput.split('-')[1];
    let nam = strInput.split('-')[2];
    return `${nam}-${thang}-${ngay}`
}
// trả về chuỗi dd-MM-yyyy với input là chuỗi
function formatStrDMY(strInput) {
    let ngay = strInput.split('-')[2];
    let thang = strInput.split('-')[1];
    let nam = strInput.split('-')[0];
    return `${ngay}-${thang}-${nam}`
}
// lấy ngày kết thúc cả tháng
function getLastDayOfMonth(year, month) {
    const nextMonth = month < 12 ? month : 1;
    const nextMonthFirstDay = new Date(year, nextMonth, 1);
    const lastDay = new Date(nextMonthFirstDay.getTime() - 1);
    return `${year}-${month.toString().padStart(2, '0')}-${lastDay.getDate()}`;
}
function getFirstDayOfMonth(year, month) {
    return `${year}-${month.toString().padStart(2, '0')}-01`;
}
// lấy ngày bắt đầu của tháng