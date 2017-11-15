import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps } from 'react-navigation';
import { WithBackButtonProps, mapBackButtonDispatchToProps } from '../layout/back-button-dispatcher';
import { Dispatch, connect } from 'react-redux';

interface VacationsScreenDispacthProps extends WithBackButtonProps {
}

const mapDipatchToProps = (dispatch: Dispatch<any>): VacationsScreenDispacthProps => ({
    ...mapBackButtonDispatchToProps(dispatch)
});

class VacationsScreenImpl extends Component<VacationsScreenDispacthProps> {
    public render() {
        return <ScrollView>
                <Text>Si!</Text>
                <Button title='Back' onPress={ this.props.onBackClick } ></Button>
            </ScrollView>;
    }
}

export const VacationsScreen = connect(undefined, mapDipatchToProps)(VacationsScreenImpl);