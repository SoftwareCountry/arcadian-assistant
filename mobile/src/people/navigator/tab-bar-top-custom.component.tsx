import React, { ReactElement } from 'react';
import { Employee } from '../../reducers/organization/employee.model';
import { Department } from '../../reducers/organization/department.model';
import { MaterialTopTabBar, NavigationRoute, TabBarTopProps, TabLabelTextParam } from 'react-navigation';
import { connect } from 'react-redux';
import { AppState } from '../../reducers/app.reducer';
import { TabBarLabel } from '../../tabbar/tab-bar-label.component';
import tabBarStyles from '../../tabbar/tab-bar-styles';
import { TextStyle } from 'react-native';

//============================================================================
type Optional<P> = P | null | undefined;

//============================================================================
interface TabBarTopCustomProps {
    employee: Employee | null;
    department: Department | null;
}

//============================================================================
class TabBarTopCustomImpl extends React.Component<TabBarTopProps & TabBarTopCustomProps> {

    //----------------------------------------------------------------------------
    public render(): React.ReactNode {
        const { employee, department } = this.props;

        return (
            <MaterialTopTabBar
                {...this.props}
                getLabelText={(param) => {
                    return this.getLabel(param, employee, department);
                }}
            />
        );
    }

    //----------------------------------------------------------------------------
    private getLabel = (param: TabLabelTextParam, employee: Optional<Employee>, department: Optional<Department>): string => {

        switch (param.route.key) {
            case 'Department':
                return department ? department.abbreviation : 'Department';
            case 'Room':
                return employee ? `Room ${employee.roomNumber}` : 'Room';
            case 'Company':
                return 'Company';
            default:
                return '';
        }
    };
}

//----------------------------------------------------------------------------
function getDepartment(state: AppState, employee: Optional<Employee>): Optional<Department> {
    if (!state.organization || !employee) {
        return null;
    }

    const { departments } = state.organization;
    return departments.find((d) => d.departmentId === employee.departmentId);
}

//----------------------------------------------------------------------------
function getEmployee(state: AppState): Optional<Employee> {
    return state.organization.employees.employeesById.get(state.userInfo.employeeId, null);
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): TabBarTopCustomProps => {
    const employee = getEmployee(state);
    const department = getDepartment(state, employee);

    return {
        employee,
        department,
    };
};

//----------------------------------------------------------------------------
export const TabBarTopCustom = connect<TabBarTopCustomProps, {}, TabBarTopProps, AppState>(stateToProps)(TabBarTopCustomImpl);
