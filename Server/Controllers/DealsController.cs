using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Relational;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities.Collections;
using ReichertsMeatDistributing.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DealsController : ControllerBase
    {

    }
}
