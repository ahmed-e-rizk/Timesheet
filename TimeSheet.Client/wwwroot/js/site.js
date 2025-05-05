 
        let currentPage = 0;
        const pageSize = 20;

        function updateCurrentTime() {
            const now = new Date();
            $('#todayDate').text(now.toLocaleDateString('en-US', {
                weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
            }));
            $('#currentTime').text(now.toLocaleTimeString('en-US', {
                hour: '2-digit', minute: '2-digit', second: '2-digit'
            }));

            const nowISO = now.toISOString().slice(0, 16);
          
        }

        setInterval(updateCurrentTime, 1000);
        updateCurrentTime();

        function formatDuration(start, end) {
            if (!start || !end) return '-';

            const diff = new Date(end) - new Date(start);
            const hours = Math.floor(diff / (1000 * 60 * 60));
            const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

            return `${hours}h ${minutes}m`;
        }

        function formatDate(dateString) {
            if (!dateString) return '-';
            const date = new Date(dateString);
            return date.toLocaleDateString();
        }

        function formatTime(timeString) {
            if (!timeString) return '-';
            const time = new Date(timeString);
            return time.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        }

function fetchData(pageNumber) {
    $.ajax({
        url: `/TimeSheetLog/GetPagedAttendanceLogs?page=${pageNumber}&size=${pageSize}`,
        method: 'GET',
        beforeSend: function () {
            $('#attendanceTable').html('<tr><td colspan="5" class="text-center">Loading data...</td></tr>');
        },
        success: function (response) { 
            const tableBody = $('#attendanceTable');
            tableBody.empty();

            if (response.data?.items && response.data.items.length > 0) {
                const items = response.data.items;
                const lastEntry = items[0];

                // last punch in , out 
                $('#lastPunchIn').text(lastEntry.loginTime ? formatTime(lastEntry.loginTime) : '-');
                $('#lastPunchOut').text(lastEntry.logoutTime ? formatTime(lastEntry.logoutTime) : '-');

                // Populate table rows
                items.forEach(item => {
                    const row = `
                        <tr>
                            <td>${item.id}</td>
                            <td>${formatDate(item.date)}</td>
                            <td>${formatTime(item.loginTime)}</td>
                            <td>${formatTime(item.logoutTime)}</td>
                            <td>${formatDuration(item.loginTime, item.logoutTime)}</td>
                        </tr>`;
                    tableBody.append(row);
                });

                renderPagination(
                    response.data.pageNumber,
                    Math.ceil(response.data.totalCount / response.data.pageSize)
                );
            } else {
                tableBody.append('<tr><td colspan="5" class="text-center">No attendance records found</td></tr>');
            }
        },
        error: function (xhr) {
            console.error("Error fetching data:", xhr.responseText);
            $('#attendanceTable').html('<tr><td colspan="5" class="text-center text-danger">Failed to load data</td></tr>');
        }
    });
}
        // Render pagination
        function renderPagination(current, totalPages) {
            const pagination = $('#pagination');
            pagination.empty();

            if (totalPages <= 1) return;

            // Previous button
            const prevDisabled = current === 1 ? 'disabled' : '';
            pagination.append(`
                <li class="page-item ${prevDisabled}">
                    <a class="page-link" href="#" onclick="changePage(${current - 1})">
                        <i class="bi bi-chevron-left"></i>
                    </a>
                </li>
            `);

            // Page numbers
            const maxVisiblePages = 5;
            let startPage = Math.max(1, current - Math.floor(maxVisiblePages / 2));
            let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

            if (endPage - startPage + 1 < maxVisiblePages) {
                startPage = Math.max(1, endPage - maxVisiblePages + 1);
            }

            if (startPage > 1) {
                pagination.append(`
                    <li class="page-item">
                        <a class="page-link" href="#" onclick="changePage(1)">1</a>
                    </li>
                    ${startPage > 2 ? '<li class="page-item disabled"><span class="page-link">...</span></li>' : ''}
                `);
            }

            for (let i = startPage; i <= endPage; i++) {
                const active = i === current ? 'active' : '';
                pagination.append(`
                    <li class="page-item ${active}">
                        <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                    </li>
                `);
            }

            if (endPage < totalPages) {
                pagination.append(`
                    ${endPage < totalPages - 1 ? '<li class="page-item disabled"><span class="page-link">...</span></li>' : ''}
                    <li class="page-item">
                        <a class="page-link" href="#" onclick="changePage(${totalPages})">${totalPages}</a>
                    </li>
                `);
            }

            // Next button
            const nextDisabled = current === totalPages ? 'disabled' : '';
            pagination.append(`
                <li class="page-item ${nextDisabled}">
                    <a class="page-link" href="#" onclick="changePage(${current + 1})">
                        <i class="bi bi-chevron-right"></i>
                    </a>
                </li>
            `);
        }

        // Handle page change
        function changePage(page) {
            if (page < 1 || page > $('#pagination li').length - 2) return;
            currentPage = page;
            fetchData(currentPage);
        }

//   call action with ajax
        function punchIn() {
            const punchTime = $('#punchInDateTime').val();
            if (!punchTime) {
                debugger;
                return;
            }

            $.ajax({
                url: '/TimeSheetLog/PunchIn',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
            PunchIn: punchTime 
        }),
                success: function(response) {
                    fetchData(currentPage);
                }
            });
        }
           //   call action with ajax
        function punchOut() {
            const punchTime = $('#punchOutDateTime').val();
            if (!punchTime) {
                return;
            }

            $.ajax({
                url: '/TimeSheetLog/PunchOut',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    PunchOut: new Date(punchTime).toISOString()
                }),
                success: function(response) {
                    fetchData(currentPage);
                },
                error: function(xhr) {
                }
            });
        }

        $(document).ready(() => {
            fetchData(currentPage);
        });
