import React, { Component } from 'react';
import { View } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';

interface EventDialogProps {
}

export class EventDialogImpl extends Component<EventDialogProps> {
    public render() {
        return (
            <View>
            </View>
        );
    }
}

const mapStateToProps = (state: AppState) => ({
});

export const EventDialog = connect(mapStateToProps)(EventDialogImpl);