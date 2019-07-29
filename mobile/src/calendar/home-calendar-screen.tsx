import React from 'react';
import { SafeAreaView, View } from 'react-native';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { CounterType, DaysCounters } from './days-counters/days-counters';
import Style from '../layout/style';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';
import { NavigationScreenProps } from 'react-navigation';
import { DepartmentFeatures } from '../reducers/user/department-features.model';
import { ActionType } from './calendar-actions-button-group';

//============================================================================
interface CalendarScreenProps {
    employee: Optional<Employee>;
    departmentFeatures: Optional<DepartmentFeatures>;
}

//============================================================================
class CalendarScreenImpl extends React.Component<CalendarScreenProps & NavigationScreenProps> {

    //----------------------------------------------------------------------------
    public static navigationOptions = {
        headerStyle: {
            backgroundColor: Style.color.base,
            height: 0,
        }
    };

    //----------------------------------------------------------------------------
    public render() {

        const { employee } = this.props;

        if (!employee) {
            return null;
        }

        return <SafeAreaView style={Style.view.safeArea}>
            <View style={Style.view.container}>
                <DaysCounters employee={employee} counters={this.counters()}/>
                <Calendar navigation={this.props.navigation}/>
                <Agenda actions={this.actions()}/>
            </View>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    private counters = (): CounterType[] => {
        let counters = [CounterType.vacation];

        const { departmentFeatures } = this.props;

        if (departmentFeatures && departmentFeatures.isDayoffsSupported) {
            counters.push(CounterType.dayoffs);
        }

        return counters;
    };

    //----------------------------------------------------------------------------
    private actions = (): ActionType[] => {
        let actions = [ActionType.vacation, ActionType.sickLeave];

        const { departmentFeatures } = this.props;

        if (departmentFeatures && departmentFeatures.isDayoffsSupported) {
            actions.push(ActionType.dayoff);
        }

        return actions;
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CalendarScreenProps => {
    const employeeId = state.userInfo ? state.userInfo.employeeId : undefined;
    const organizationEmployeeId = state.organization && employeeId ? state.organization.employees.employeesById.get(employeeId) : undefined;
    const departmentFeatures = state.userInfo ? state.userInfo.userDepartmentFeatures : undefined;

    return {
        employee: organizationEmployeeId,
        departmentFeatures: departmentFeatures,
    };
};

//----------------------------------------------------------------------------
export const CalendarScreenComponent = connect(stateToProps)(CalendarScreenImpl);
