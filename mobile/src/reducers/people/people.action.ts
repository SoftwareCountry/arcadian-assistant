export interface SelectCompanyDepartment {
    type: 'SELECT-COMPANY-DEPARTMENT';
    departmentId: string;
    staffDepartmentId: string;
}

export const selectCompanyDepartment = (departmentId: string, staffDepartmentId: string): SelectCompanyDepartment => 
    ({ type: 'SELECT-COMPANY-DEPARTMENT', departmentId, staffDepartmentId });

export interface RedirectToEmployeeDetails {
    type: 'REDIRECT-TO-EMPLOYEE-DETAILS';
    employeeId: string;
}

export const redirectToEmployeeDetails = (employeeId: string): RedirectToEmployeeDetails => 
    ({ type: 'REDIRECT-TO-EMPLOYEE-DETAILS', employeeId });

export type PeopleActions = SelectCompanyDepartment | RedirectToEmployeeDetails;
