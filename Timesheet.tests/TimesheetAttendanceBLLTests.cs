using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Timesheet.BLL.TimesheetLog;
using Timesheet.Core.Entites;
using Timesheet.DTO.TimesheetLog;
using Timesheet.Repositroy.Infrastructure;
using Repositroy;

public class TimesheetAttendanceBLLTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepository<AttendanceLogs>> _attendanceLogsRepoMock = new();

    private TimesheetAttendanceBLL CreateSut() =>
        new TimesheetAttendanceBLL(_unitOfWorkMock.Object, _mapperMock.Object, _attendanceLogsRepoMock.Object);

    [Fact]
    public async Task GetTodayAttendance_ShouldReturnLog_WhenExists()
    {
        // Arrange
        var userId = 1;
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var log = new AttendanceLogs { UserId = userId, Date = today };
        var logDto = new AttendanceLogsDto { Id = userId };

        _attendanceLogsRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AttendanceLogs, bool>>>()))
                               .ReturnsAsync(log);
        _mapperMock.Setup(x => x.Map<AttendanceLogsDto>(log)).Returns(logDto);

        var sut = CreateSut();

        // Act
        var response = await sut.GetTodayAttendance(userId);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.Equal(userId, response.Data.Id);
    }

    [Fact]
    public async Task SubmitPunchInTime_ShouldCreateNewLog_WhenNoneExists()
    {
        // Arrange
        var userId = 1;
        var punchIn = DateTime.UtcNow;

        _attendanceLogsRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AttendanceLogs, bool>>>()))
                               .ReturnsAsync((AttendanceLogs)null!);
        _attendanceLogsRepoMock.Setup(x => x.AddAsync(It.IsAny<AttendanceLogs>()))
                         .ReturnsAsync((AttendanceLogs a) => a);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

        var sut = CreateSut();

        // Act
        var response = await sut.SubmitPunchInTime(userId, punchIn);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task SubmitPunchOutTime_ShouldUpdateLog_WhenExists()
    {
        // Arrange
        var userId = 1;
        var punchOut = DateTime.UtcNow;
        var existingLog = new AttendanceLogs { UserId = userId, Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        _attendanceLogsRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AttendanceLogs, bool>>>()))
                               .ReturnsAsync(existingLog);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

        var sut = CreateSut();

        // Act
        var response = await sut.SubmitPunchOutTime(userId, punchOut);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.True(response.Data);
        Assert.Equal(punchOut, existingLog.LogoutTime);
    }

    [Fact]
    public async Task GetAllAttendance_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var size = 2;

        var sampleList = new List<AttendanceLogs>
        {
            new AttendanceLogs { UserId = userId },
            new AttendanceLogs { UserId = userId }
        };

        _attendanceLogsRepoMock.Setup(x => x.Where(It.IsAny<Expression<Func<AttendanceLogs, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
                         .Returns(sampleList.AsQueryable());

        _attendanceLogsRepoMock
    .Setup(x => x.Where(It.IsAny<Expression<Func<AttendanceLogs, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
    .Returns(sampleList.AsQueryable());
        _mapperMock.Setup(x => x.Map<List<AttendanceLogsDto>>(sampleList))
                   .Returns(new List<AttendanceLogsDto> { new(), new() });

        var sut = CreateSut();

        // Act
        var response = await sut.GetAllAttendance(userId, page, size);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Items.Count);
        Assert.Equal(2, response.Data.TotalCount);
    }
}
