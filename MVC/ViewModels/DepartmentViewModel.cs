using Domain.Models;


namespace MVC.ViewModels
{
    public class DepartmentViewModel
    {
        public DepartmentViewModel()
        {
            ConnectedSubdepartmentsNames = new List<string>();
            DisconnectedSubdepartmentsNames = new List<string>();
        }

        public DepartmentViewModel(Department department, ref List<string> allDepartmentsNames) : this()
        {
            Department = department;
            ConnectedSubdepartmentsNames = (from d in department.Subdepartments
                                            select d.Name).ToList();

            List<string> exclude = new();
            exclude.Add(department.Name);

            foreach (string departmentName in ConnectedSubdepartmentsNames)
            {
                exclude.Add(departmentName);
            }

            allDepartmentsNames.RemoveAll(depName => exclude.Any(exc => exc == depName)); // filter

            DisconnectedSubdepartmentsNames = allDepartmentsNames; // filtered
        }


        public Department Department { get; set; }

        public IList<string> ConnectedSubdepartmentsNames { get; set; }

        public IList<string> DisconnectedSubdepartmentsNames { get; set; }
    }
}
