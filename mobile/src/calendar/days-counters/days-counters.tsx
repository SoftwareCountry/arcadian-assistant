import React, { Component } from 'react';
import { StyleProp, View, ViewStyle } from 'react-native';
import { daysCountersStyles } from './styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { HoursCreditCounter, VacationDaysCounter } from '../../reducers/calendar/days-counters.model';
import { LoadingView } from '../../navigation/loading';
import { ConvertHoursCreditToDays } from '../../reducers/calendar/convert-hours-credit-to-days';
import { Employee } from '../../reducers/organization/employee.model';

interface DaysCountersProps {
    employee: Employee;
    additionalStyle?: StyleProp<ViewStyle>;
}

export class DaysCounters extends Component<DaysCountersProps> {

    public render() {
        const { vacationDaysLeft, hoursCredit} = this.props.employee;
        if (!vacationDaysLeft && !hoursCredit) {
            return (
                <View style={daysCountersStyles.container}>
                    <LoadingView/>
                </View>
            );
        }

        const allVacationDaysCounter = new VacationDaysCounter(vacationDaysLeft);

        const daysConverter = new ConvertHoursCreditToDays();
        const calculatedDays = daysConverter.convert(hoursCredit);

        const hoursCreditCounter = new HoursCreditCounter(hoursCredit, calculatedDays.days, calculatedDays.rest);

        const vacationCounter = allVacationDaysCounter
            ? <DaysCounter  textValue={allVacationDaysCounter.toString()}
                            title={allVacationDaysCounter.title}
                            icon={{
                                name: 'vacation',
                                size: 30
                            }} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCreditCounter
            ? <DaysCounter  textValue={hoursCreditCounter.toString()}
                            title={hoursCreditCounter.title}
                            icon={{
                                name: 'dayoff',
                                size: 30
                            }} />
            : <EmptyDaysCounter />;

        return (
            <View style={this.containerStyle()}>
                    { vacationCounter }
                    { daysoffCounter }
            </View>
        );
    }

    private containerStyle = (): StyleProp<ViewStyle> => {
            return [daysCountersStyles.container, this.props.additionalStyle];
    };
}
