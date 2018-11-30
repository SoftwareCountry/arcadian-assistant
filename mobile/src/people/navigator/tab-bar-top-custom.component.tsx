import React, { ReactElement } from 'react';
import { Employee } from '../../reducers/organization/employee.model';
import { Department } from '../../reducers/organization/department.model';
import { TabBarTop, TabBarTopProps, TabScene } from 'react-navigation';
import { connect } from 'react-redux';
import { AppState } from '../../reducers/app.reducer';
import { Platform } from 'react-native';
import { StyledText } from '../../override/styled-text';
import tabBarStyles from '../../tabbar/tab-bar-styles';
import { TabBarLabel } from '../../tabbar/tab-bar-label.component';

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
            <TabBarTop
                {...this.props}
                getLabel={(scene) => {
                    return this.getLabel(scene, employee, department);
                }}
            />
        );
    }

    //----------------------------------------------------------------------------
    private getLabel = (scene: TabScene, employee: Optional<Employee>, department: Optional<Department>): React.ReactNode | string => {
        switch (scene.index) {
            case 0:
                return this.styleTabBarLabel(department ? department.abbreviation : 'Department');
            case 1:
                return this.styleTabBarLabel(employee ? `Room ${employee.roomNumber}` : 'Room');
            case 2:
                return this.styleTabBarLabel('Company');
            default:
                return '';
        }
    };

    //----------------------------------------------------------------------------
    private styleTabBarLabel = (label: string): string | ReactElement<any> => {
        return <TabBarLabel label={label}/>;
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
