import React from 'react';
import { SafeAreaView, View } from 'react-native';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { DaysCounters } from './days-counters/days-counters';
import Style from '../layout/style';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { Nullable, Optional } from 'types';

//============================================================================
interface CalendarScreenProps {
    employee: Optional<Employee>;
}

//============================================================================
class CalendarScreenImpl extends React.Component<CalendarScreenProps> {

    //----------------------------------------------------------------------------
    public static navigationOptions = {
        headerStyle: {
            backgroundColor: Style.color.base
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
                <DaysCounters employee={employee}/>
                <Calendar/>
                <Agenda/>
            </View>
        </SafeAreaView>;
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CalendarScreenProps => {
    const employeeId = state.userInfo ? state.userInfo.employeeId : null;
    const organizationEmployeeId = state.organization && employeeId ?
        state.organization.employees.employeesById.get(employeeId) : null;

    return {
        employee: employeeId == null ? null : organizationEmployeeId,
    };
};

//----------------------------------------------------------------------------
export const CalendarScreenComponent = connect(stateToProps)(CalendarScreenImpl);
