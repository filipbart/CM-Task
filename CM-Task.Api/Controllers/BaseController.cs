using Microsoft.AspNetCore.Mvc;

namespace CM_Task.Api.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase;