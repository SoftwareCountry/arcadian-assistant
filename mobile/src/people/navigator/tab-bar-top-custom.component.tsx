import React from 'react';
import { Employee } from '../../reducers/organization/employee.model';
import { Department } from '../../reducers/organization/department.model';
import { MaterialTopTabBar, TabBarTopProps, TabLabelTextParam } from 'react-navigation';
import { connect } from 'react-redux';
import { AppState } from '../../reducers/app.reducer';
import { Nullable, Optional } from 'types';
import { getEmployee } from '../../utils/utils';
import { toNullable } from '../../types/types-utils';

//============================================================================
interface TabBarTopCustomProps {
    employee: Nullable<Employee>;
    department: Nullable<Department>;
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
    private getLabel = (param: TabLabelTextParam, employee: Nullable<Employee>, department: Nullable<Department>): string => {

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
        return undefined;
    }

    const { departments } = state.organization;
    const department = departments.find((d) => d.departmentId === employee.departmentId);
    return department ? department : undefined;
}

///----------------------------------------------------------------------------
const stateToProps = (state: AppState): TabBarTopCustomProps => {
    const employee = getEmployee(state);
    const department = getDepartment(state, employee);

    return {
        employee: toNullable(employee),
        department: toNullable(department),
    };
};

//----------------------------------------------------------------------------
export const TabBarTopCustom = connect<TabBarTopCustomProps, {}, TabBarTopProps, AppState>(stateToProps)(TabBarTopCustomImpl);
