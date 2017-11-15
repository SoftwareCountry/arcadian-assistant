import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import { Dispatch, connect } from 'react-redux';
import { WithBackButtonProps, mapBackButtonDispatchToProps } from '../layout/back-button-dispatcher';

interface DaysoffScreenDispatchProps extends WithBackButtonProps {
}

const mapDispatchToProps = (dispatch: Dispatch<any>): DaysoffScreenDispatchProps => ({
    ...mapBackButtonDispatchToProps(dispatch)
});

export class DaysoffScreenImpl extends Component<DaysoffScreenDispatchProps> {
    public render() {
        return <ScrollView>
                <Text>This is not the screen you're looking for</Text>
                <Button title='Back' onPress={ () => this.props.onBackClick } ></Button>
            </ScrollView>;
    }
}

export const DaysoffScreen = connect(undefined, mapDispatchToProps)(DaysoffScreenImpl);