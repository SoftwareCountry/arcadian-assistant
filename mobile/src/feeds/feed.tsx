import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image, LayoutChangeEvent, TouchableOpacity } from 'react-native';
import { LayoutEvent } from 'react-navigation';

import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { Dispatch, connect } from 'react-redux';

interface FeedListItemProps {
    message: Feed;
    employee: Employee;
    onAvatarClicked: (e: Employee) => void;
}

interface FeedListItemState {
    avatarHeight: number;
}

export class FeedListItem extends React.Component<FeedListItemProps, FeedListItemState> {
    constructor(props: FeedListItemProps) {
        super(props);
        this.state = {
            avatarHeight: undefined
        };
    }

    public render() {
        const message = this.props.message;
        const employee = this.props.employee;
        const photo = employee ? employee.photoUrl : null;
        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        const imgContainerStyle = StyleSheet.flatten([
            styles.imgContainer,
            this.state.avatarHeight
                ? { height: this.state.avatarHeight }
                : {}
        ]);
        const isDisabledClick = (this.props.employee) ? false : true;

        const avatarContent = isDisabledClick ?
            <Avatar photoUrl={photo} /> :
            <TouchableOpacity onPress={this.onAvatarClicked} style={styles.touchableOpacityContainer} >
                <Avatar photoUrl={photo} />
            </TouchableOpacity> ;

        return (
            <View>
                <View style={styles.layout}>
                    <View style={imgContainerStyle} onLayout={this.onAvatarContainerLayout}>
                        {avatarContent}
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.title}>{this.props.message.title}</StyledText>
                        <StyledText>
                            <Text style={styles.text}>{this.props.message.text}</Text>
                        </StyledText>
                        <StyledText style={styles.tags}>#ArcadiaNews</StyledText>
                        <StyledText style={styles.date}>{formattedDate}</StyledText>
                    </View>
                </View>
            </View>
        );
    }

    private onAvatarContainerLayout = (evt: LayoutChangeEvent) => {
        const layout = evt.nativeEvent.layout;
        this.setState({
            avatarHeight: Math.max(layout.height, layout.width)
        });
    }
    private onAvatarClicked = () => {
        this.props.onAvatarClicked(this.props.employee);
    }
}
