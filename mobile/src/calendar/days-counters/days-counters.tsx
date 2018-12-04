import React, { Component } from 'react';
import { StyleProp, View, ViewStyle } from 'react-native';
import { StyleDays } from './styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { HoursCreditCounter, VacationDaysCounter } from '../../reducers/calendar/days-counters.model';
import { LoadingView } from '../../navigation/loading';
import { ConvertHoursCreditToDays } from '../../reducers/calendar/convert-hours-credit-to-days';
import { Employee } from '../../reducers/organization/employee.model';
import { none } from '../../types/types-utils';
import { Nullable } from 'types';

//============================================================================
interface DaysCountersProps {
    employee: Employee;
    additionalStyle?: StyleProp<ViewStyle>;
}

//============================================================================
export class DaysCounters extends Component<DaysCountersProps> {

    //----------------------------------------------------------------------------
    public render() {
        if (this.props.employee.vacationDaysLeft === null ||
            this.props.employee.hoursCredit === null) {
            return (
                <View style={StyleDays.counters.container}>
                    <LoadingView/>
                </View>
            );
        }

        const { vacationDaysLeft, hoursCredit } = this.props.employee;

        return (
            <View style={this.containerStyle()}>
                {this.renderVacationCounter(vacationDaysLeft)}
                {this.renderDaysOffCounter(hoursCredit)}
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private containerStyle = (): StyleProp<ViewStyle> => {
        return [StyleDays.counters.container, this.props.additionalStyle];
    };

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private renderVacationCounter(vacationDaysLeft: Nullable<number>): React.ReactNode {
        if (none(vacationDaysLeft)) {
            return <EmptyDaysCounter/>;
        }

        const allVacationDaysCounter = new VacationDaysCounter(vacationDaysLeft);
        return <DaysCounter textValue={allVacationDaysCounter.toString()}
                            title={allVacationDaysCounter.title}
                            icon={{ name: 'vacation', size: 30 }}/>;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private renderDaysOffCounter(hoursCredit: Nullable<number>): React.ReactNode {
        if (none(hoursCredit)) {
            return <EmptyDaysCounter/>;
        }

        const daysCredit = new ConvertHoursCreditToDays().convert(hoursCredit);
        const hoursCreditCounter = new HoursCreditCounter(hoursCredit, daysCredit.days, daysCredit.rest);

        return <DaysCounter textValue={hoursCreditCounter.toString()}
                           title={hoursCreditCounter.title}
                           icon={{ name: 'dayoff', size: 30 }}/>;
    }
}
