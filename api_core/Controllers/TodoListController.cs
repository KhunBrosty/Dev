using System.Data;
using API_Core.Modles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace API_Core.Controllers
{
    [Route("home/todolist")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly ILogger<TodoListController> _logger;
        private readonly string _connectionString;
        public TodoListController(ILogger<TodoListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public async Task<List<TodoModle>> GetTodoList()
        {
            List<TodoModle> ls = new List<TodoModle>();
            using (var con = new OracleConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM TODO_LIST";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        foreach (DataRow row in dt.Rows)
                        {
                            ls.Add(new TodoModle
                            {
                                Id = row["ID"].ToString(),
                                Title = row["TITLE"].ToString(),
                                Description = row["DESCRIPTION"].ToString(),
                                CreateDate = row["CREATE_DATE"].ToString(),
                                Priority = row["PRIORITY"].ToString(),
                                IsCompleted = row["ISCOMPLETED"].ToString()

                            });
                        }
                    }
                }
            }
            return ls;
        }

        [HttpPost]
        public async Task<IActionResult> AddTodoList(TodoModle todo)
        {
            using (var con = new OracleConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO TODO_LIST (ID, TITLE, DESCRIPTION, PRIORITY, ISCOMPLETED) VALUES (:id, :title, :description, :priority, :iscompleted)";
                    cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = todo.Id;
                    cmd.Parameters.Add("title", OracleDbType.Varchar2).Value = todo.Title;
                    cmd.Parameters.Add("description", OracleDbType.Varchar2).Value = todo.Description;
                    cmd.Parameters.Add("priority", OracleDbType.Varchar2).Value = todo.Priority;
                    cmd.Parameters.Add("iscompleted", OracleDbType.Varchar2).Value = todo.IsCompleted;

                    await cmd.ExecuteReaderAsync();
                }
            }
            return Ok();
        }
    }
}
