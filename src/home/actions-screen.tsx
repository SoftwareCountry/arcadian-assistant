import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';

const mapStateToProps = (state: AppState, ownProps: NavigationScreenProps<{}>) => ({
    ...ownProps,
    messages: state.messages
});

class ActionsScreenImpl extends Component<NavigationScreenProps<{messages: any}>> {
    public render() {

        console.log(this.props);

        return <ScrollView>
                <Text>Si!</Text>
                <Button title='Back' onPress={ () => this.props.navigation.goBack() } ></Button>
            </ScrollView>;
    }
}

export const ActionsScreen = connect(mapStateToProps)(ActionsScreenImpl as any);