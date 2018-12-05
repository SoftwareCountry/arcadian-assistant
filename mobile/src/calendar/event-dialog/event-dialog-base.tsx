import React, { Component } from 'react';
import { layout, content, buttons } from './styles';
import { View, TouchableOpacity, ActivityIndicator, ViewStyle, TextStyle } from 'react-native';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';
import { CalendarActionButton } from '../calendar-action-button';
import { connect, MapStateToProps } from 'react-redux';
import { AppState } from '../../reducers/app.reducer';

interface EventDialogBaseDefaultProps {
    disableAccept?: boolean;
}

interface EventDialogBaseOwnProps extends EventDialogBaseDefaultProps {
    icon: string;
    title: string;
    text: string;
    cancelLabel?: string;
    acceptLabel: string;
    onCancelPress?: () => void;
    onAcceptPress: () => void;
    onClosePress: () => void;
}

interface EventDialogMappedProps {
    inProgress: boolean;
}

type EventDialogBaseProps = EventDialogBaseOwnProps & EventDialogMappedProps;

export const eventDialogTextDateFormat = 'MMMM D, YYYY';

export class EventDialogBaseImpl extends Component<EventDialogBaseProps> {
    public static defaultProps: EventDialogBaseDefaultProps = {
        disableAccept: false
    };

    public render() {
        const actionButtons = this.getActionButtons();

        const { icon, title, text } = this.props;

        return (
            <View style={layout.container}>
                <View style={layout.icon}>
                    <ApplicationIcon name={this.props.icon} style={content.icon} />
                </View>
                <View style={layout.content}>
                    <View style={layout.text}>
                        <StyledText style={content.title}>{title}</StyledText>
                        <View>
                            <StyledText style={content.text}>{text}</StyledText>
                        </View>
                    </View>
                    {this.props.children}
                    {actionButtons}
                </View>
                <View style={layout.close}>
                    <TouchableOpacity onPress={this.close}>
                        <StyledText style={buttons.close}>˟</StyledText>
                    </TouchableOpacity>
                </View>
            </View>
        );
    }

    private getActionButtons() {
        const { acceptLabel, disableAccept } = this.props;
        return (
            <View style={layout.buttons}>
                {this.getCancelButton()}
                <CalendarActionButton title={acceptLabel} onPress={this.accept}
                                      style={buttons.accept as ViewStyle}
                                      textStyle={buttons.acceptLabel as TextStyle}
                                      disabled={disableAccept || this.props.inProgress}/>
                {this.getActivityIndicator()}
            </View>
        );
    }

    private getCancelButton() {
        const { cancelLabel } = this.props;
        if (!cancelLabel) {
            return null;
        }
        return <CalendarActionButton title={cancelLabel} onPress={this.cancel}
                                     style={buttons.cancel as ViewStyle}
                                     textStyle={buttons.cancelLabel as TextStyle}
                                     disabled={this.props.inProgress}/>;
    }

    private getActivityIndicator() {
        if (!this.props.inProgress) {
            return null;
        }
        return <ActivityIndicator size={'small'} style={buttons.progressIndicator}/>;
    }

    private close = () => {
        this.props.onClosePress();
    };

    private cancel = () => {
        if (!this.props.onCancelPress) {
            return;
        }
        this.props.onCancelPress();
    };

    private accept = () => {
        this.props.onAcceptPress();
    };
}

const mapStateToProps: MapStateToProps<EventDialogBaseProps, EventDialogBaseOwnProps, AppState> = (state, ownProps) => ({
    inProgress: state.calendar ? state.calendar.eventDialog.inProgress : false,
    icon: ownProps.icon,
    title: ownProps.title,
    text: ownProps.text,
    cancelLabel: ownProps.cancelLabel,
    acceptLabel: ownProps.acceptLabel,
    onCancelPress: ownProps.onCancelPress,
    onAcceptPress: ownProps.onAcceptPress,
    onClosePress: ownProps.onClosePress
});

export const EventDialogBase = connect(mapStateToProps)(EventDialogBaseImpl);
