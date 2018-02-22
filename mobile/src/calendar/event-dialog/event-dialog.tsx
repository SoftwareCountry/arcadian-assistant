import React, { Component } from 'react';
import { View, TouchableOpacity } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';
import { CalendarActions, cancelDialog } from '../../reducers/calendar/calendar.action';

export interface EventDialogProps {
    title: string;
    text: string;
    icon: string;
}

interface EventDialogDispatchProps {
    cancel: () => void;
}

export class EventDialogImpl extends Component<EventDialogProps & EventDialogDispatchProps> {
    public render() {
        const iconSize = 0;

        return (
            <View>
                <View>
                    <ApplicationIcon name={this.props.icon} size={iconSize} />
                </View>
                <View>
                    <StyledText>{this.props.title}</StyledText>
                    <View>
                        <StyledText>{this.props.text}</StyledText>
                    </View>

                    <View>
                        {/* Button 1 */}
                        {/* Button 2 */}
                    </View>
                </View>
                <View>
                    <TouchableOpacity onPress={this.cancel}>
                        <StyledText style={{fontSize: 50}}>˟</StyledText>
                    </TouchableOpacity>
                </View>
            </View>
        );
    }

    private cancel = () => {
        this.props.cancel();
    }
}

const mapStateToProps = (state: AppState) => ({
    title: state.calendar.calendarEvents.dialog.title,
    text: state.calendar.calendarEvents.dialog.text,
    icon: state.calendar.calendarEvents.dialog.icon
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    cancel: () => {dispatch(cancelDialog());}
});

export const EventDialog = connect(mapStateToProps, mapDispatchToProps)(EventDialogImpl);