import React, { Component } from 'react';
import { StyleProp, View, ViewStyle } from 'react-native';
import { daysCountersStyles } from './styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { LoadingView } from '../../navigation/loading';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
}

interface ExplicitDaysCountersProps {
    explicitCounters?: DaysCountersModel;
    explicitStyle?: StyleProp<ViewStyle>;
}

class DaysCountersImpl extends Component<DaysCountersProps & ExplicitDaysCountersProps> {

    public render() {

        const shouldUseExplicitValues = this.props.explicitCounters &&
            (this.props.explicitCounters.hoursCredit || this.props.explicitCounters.allVacationDays);

        const { allVacationDays, hoursCredit } = shouldUseExplicitValues ?
            this.props.explicitCounters :
            this.props.daysCounters;

        if (!shouldUseExplicitValues && !allVacationDays && !hoursCredit) {
            return (
                <View style={daysCountersStyles.container}>
                    <LoadingView/>
                </View>
            );
        }

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title}
                            icon={{
                                name: 'vacation',
                                size: 30
                            }} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title}
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
            return [daysCountersStyles.container, this.props.explicitStyle];
    };
}

const mapStateToProps = (state: AppState, ownProps: ExplicitDaysCountersProps): DaysCountersProps & ExplicitDaysCountersProps => ({
    daysCounters: state.calendar.daysCounters,
    ...ownProps,
});

export const DaysCounters = connect(mapStateToProps)(DaysCountersImpl);
