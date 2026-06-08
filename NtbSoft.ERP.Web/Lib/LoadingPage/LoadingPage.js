
function showLoading() {
    document.querySelector('.loading').classList.add('active'); // Add 'active' class to show loading
}

function hideLoading() {
    setTimeout(function () {
        document.querySelector('.loading').classList.remove('active');
    }, 200);
}

function Loading(times) {
    showLoading();
    setTimeout(function () {

        hideLoading();
    }, times);
}