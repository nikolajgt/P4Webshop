namespace P4Webshop.Models.DTO
{
    public class AllUserTypesDTO
    {
        public List<Customer> Customers { get; set; }
        public List<Admin> Admins { get; set; }
        public List<Employee> Employees { get; set; }

        public AllUserTypesDTO(List<Customer> customers, List<Admin> admins, List<Employee> employees)
        {
            Customers = customers;
            Admins = admins;
            Employees = employees;
        }
    }
}
