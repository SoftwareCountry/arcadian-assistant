import React, { Component } from 'react';
import { TouchableOpacity } from 'react-native';
import { connect } from 'react-redux';
import { ApplicationIcon } from '../override/application-icon';
import { preferencesStyles } from './preferences.styles';
import { Action, Dispatch } from 'redux';
import { openUserPreferences } from '../navigation/navigation.actions';

interface SettingsDispatchProps {
    onSettingsClicked: () => void;
}

const mapDispatchToProps = (dispatch: Dispatch<Action>): SettingsDispatchProps => ({
    onSettingsClicked: () => {
        dispatch(openUserPreferences());
    }
});

class SettingsViewImpl extends Component<SettingsDispatchProps> {
    public render() {
        return <TouchableOpacity onPress={this.props.onSettingsClicked} style={preferencesStyles.settingsView}>
            <ApplicationIcon
                name={'settings'}
                style={preferencesStyles.icon}/>
        </TouchableOpacity>;
    }
}

export const SettingsView = connect(null, mapDispatchToProps)(SettingsViewImpl);
